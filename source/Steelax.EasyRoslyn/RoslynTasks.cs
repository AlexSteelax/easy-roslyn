using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Steelax.EasyRoslyn.Common;
using Steelax.EasyRoslyn.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Steelax.EasyRoslyn
{
    public static class RoslynTasks
    {
        public delegate T Configure<T>(T settings);

        /// <summary>
        /// Emit the IL for the compiled source code
        /// </summary>
        /// <param name="syntaxTrees"></param>
        /// <param name="referenceConfigurator"></param>
        /// <param name="compilationConfigurator"></param>
        /// <returns></returns>
        public static RoslynCompileResult RoslynCompile(IEnumerable<SyntaxTree> syntaxTrees, Configure<ReferenceSettings> referenceConfigurator = null, Configure<CompilationSettings> compilationConfigurator = null)
        {
            var referenceSettings = referenceConfigurator == null ? new ReferenceSettings() : referenceConfigurator(new ReferenceSettings());
            var compilationSettings = compilationConfigurator == null ? new CompilationSettings() : compilationConfigurator(new CompilationSettings());

            var references = new List<MetadataReference>
            {
                typeof(object).Assembly.GetMetadataReference()
            };

            foreach (var assembly in referenceSettings.AssemblyCollection)
                references.Add(assembly.GetMetadataReference());

            var compilation = CSharpCompilation.Create(compilationSettings.AssemblyName, syntaxTrees, references, compilationSettings.CompilationOptions);

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);

                ms.Seek(0, SeekOrigin.Begin);

                return new RoslynCompileResult
                {
                    Assembly = result.Success ? ms.ToArray() : null,
                    AssemblyName = compilationSettings.AssemblyName,
                    Diagnostics = result.Diagnostics,
                    Success = result.Success
                };
            }
        }

        /// <summary>
        /// Emit the IL for the compiled source code
        /// </summary>
        /// <param name="syntaxTree"></param>
        /// <param name="referenceConfigurator"></param>
        /// <param name="compilationConfigurator"></param>
        /// <returns></returns>
        public static RoslynCompileResult RoslynCompile(SyntaxTree syntaxTree, Configure<ReferenceSettings> referenceConfigurator = null, Configure<CompilationSettings> compilationConfigurator = null)
        {
            return RoslynCompile(new[] { syntaxTree }, referenceConfigurator, compilationConfigurator);
        }

        /// <summary>
        /// Formats and writes diagnostic messages
        /// </summary>
        /// <param name="compileResult"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static RoslynCompileResult WriteLog(this RoslynCompileResult compileResult, ILogger logger)
        {
            foreach (var diagnostic in compileResult.Diagnostics)
            {
                var logLevel =
                    (diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error) ? LogLevel.Error :
                    (diagnostic.Severity == DiagnosticSeverity.Warning) ? LogLevel.Warning :
                    (diagnostic.Severity == DiagnosticSeverity.Info) ? LogLevel.Information :
                    LogLevel.Debug;

                var id = diagnostic.Id;
                var message = diagnostic.GetMessage();

                logger.Log(logLevel, "{id}: {message}", id, message);
            }

            return compileResult;
        }

        /// <summary>
        /// Load the assembly into the application domain
        /// </summary>
        /// <param name="compileResult"></param>
        /// <param name="assembly">The loaded assembly</param>
        /// <returns></returns>
        public static RoslynCompileResult TryLoad(this RoslynCompileResult compileResult, out Assembly assembly)
        {
            assembly = compileResult.Success ? Assembly.Load(compileResult.Assembly) : null;

            return compileResult;
        }

        /// <summary>
        /// Create a new file, writes the specified assembly byte array th the file
        /// </summary>
        /// <param name="compileResult"></param>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static RoslynCompileResult TrySave(this RoslynCompileResult compileResult, string directory)
        {
            if (compileResult.Success)
            {
                File.WriteAllBytes(Path.Combine(directory, $"{compileResult.AssemblyName}.dll"), compileResult.Assembly);
            }
            
            return compileResult;
        }
    }
}
