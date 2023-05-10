using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using Steelax.EasyRoslyn.Common;
using Steelax.EasyRoslyn.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Steelax.EasyRoslyn.CodeAnalysis
{
    public abstract partial class CodeBuilder<TSelf, TCompilationOptions, TParseOptions>
        where TSelf: class
        where TCompilationOptions : CompilationOptions
        where TParseOptions: ParseOptions
    {
        private ReferenceBag refList;
        private SourceBag srcList;
        private TCompilationOptions cmpOptions;
        private TParseOptions parseOptions;
        private EmitOptions emitOptions;

        private string assemblyName;

        internal CodeBuilder(TCompilationOptions cmpOptions, TParseOptions parseOptions)
        {
            this.cmpOptions = cmpOptions;
            this.parseOptions = parseOptions;

            refList = new();
            srcList = new();

            emitOptions = new();

            assemblyName = Path.GetRandomFileName();
        }

        #region Configure methods
        /// <summary>
        /// Configure emit options
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public TSelf ConfigureEmitOptions(Func<EmitOptions, EmitOptions> configure)
        {
            emitOptions = configure(emitOptions);
            return this as TSelf;
        }
        /// <summary>
        /// Configure compilation options
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public TSelf ConfigureCompilationOptions(Func<TCompilationOptions, TCompilationOptions> configure)
        {
            cmpOptions = configure(cmpOptions);
            return this as TSelf;
        }
        /// <summary>
        /// Configure file parse options
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public TSelf ConfigureParseOptions(Func<TParseOptions, TParseOptions> configure)
        {
            parseOptions = configure(parseOptions);
            return this as TSelf;
        }
        /// <summary>
        /// Configure code sources
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public TSelf ConfigureSources(Action<SourceBag> configure)
        {
            configure(srcList);
            return this as TSelf;
        }
        /// <summary>
        /// Configure used references
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public TSelf ConfigureReferences(Action<ReferenceBag> configure)
        {
            configure(refList);
            return this as TSelf;
        }
        /// <summary>
        /// Define assembly name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TSelf SetAssemblyName(string name)
        {
            assemblyName = name;
            return this as TSelf;
        }
        #endregion
        #region Configured parameters
        /// <summary>
        /// Get syntax trees from sources
        /// </summary>
        protected IEnumerable<SyntaxTree> SyntaxTrees
        {
            get
            {
                return srcList.Select(o => Parse(o.SourceText, o.Path));
            }
        }
        /// <summary>
        /// Get embedded text (pdb) from sources
        /// </summary>
        protected IEnumerable<EmbeddedText> EmbeddedTexts
        {
            get
            {
                return srcList.Select(o => EmbeddedText.FromSource(o.Path, o.SourceText));
            }
        }
        /// <summary>
        /// Get references
        /// </summary>
        protected IEnumerable<MetadataReference> References
        {
            get
            {
                yield return typeof(object).Assembly.GetMetadataReference();

                foreach (var assembly in refList)
                    yield return assembly.GetMetadataReference();
            }
        }
        /// <summary>
        /// Get assembly name
        /// </summary>
        protected string AssemblyName
        {
            get
            {
                return assemblyName;
            }
        }
        /// <summary>
        /// Get compilation options
        /// </summary>
        protected TCompilationOptions CompilationOptions
        {
            get
            {
                return cmpOptions;
            }
        }
        /// <summary>
        /// Get source file parse options
        /// </summary>
        protected TParseOptions ParseOptions
        {
            get
            {
                return parseOptions;
            }
        }
        #endregion

        protected abstract SyntaxTree Parse(SourceText source, string path);
        protected abstract Compilation CreateCompilation();

        /// <summary>
        /// Compile and build result
        /// </summary>
        /// <returns></returns>
        public CompileResult Build()
        {
            var compilation = CreateCompilation();

            using var assemblyStream = new MemoryStream();
            using var symbolsStream = new MemoryStream();

            var result = compilation.Emit(
                peStream: assemblyStream,
                pdbStream: symbolsStream,
                embeddedTexts: EmbeddedTexts,
                options: emitOptions);

            assemblyStream?.Seek(0, SeekOrigin.Begin);
            symbolsStream?.Seek(0, SeekOrigin.Begin);

            if (!result.Success)
            {
                return new CompileResult
                {
                    Diagnostics = result.Diagnostics,
                    Success = result.Success
                };
            }

            return new CompileResult
            {
                Assembly = assemblyStream.ToArray(),
                Symbols = symbolsStream?.ToArray(),
                AssemblyName = AssemblyName,
                Diagnostics = result.Diagnostics,
                Success = result.Success
            };
        }
    }
}
