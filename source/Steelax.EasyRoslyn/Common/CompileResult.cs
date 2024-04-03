using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Steelax.EasyRoslyn.Exceptions;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Reflection;

namespace Steelax.EasyRoslyn.Common
{
    public class CompileResult
    {
        public byte[]? Assembly { get; internal set; }
        public byte[]? Symbols { get; internal set; }
        public string AssemblyName { get; internal set; } = null!;
        public ImmutableArray<Diagnostic> Diagnostics { get; internal set; }
        public bool Success { get; internal set; }

        internal CompileResult() { }

        /// <summary>
        /// Formats and writes diagnostic messages
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public CompileResult WriteLog(ILogger logger)
        {
            foreach (var diagnostic in Diagnostics)
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

            return this;
        }

        /// <summary>
        /// Throw EmitException if not success
        /// </summary>
        /// <returns></returns>
        /// <exception cref="EmitException"></exception>
        public CompileResult ThrowIfNotSuccess()
        {
            if (!Success)
                throw new EmitException();

            return this;
        }

        /// <summary>
        /// Load the assembly into the application domain
        /// </summary>
        /// <param name="assembly">The loaded assembly</param>
        /// <returns></returns>
        public CompileResult TryLoad(out Assembly? assembly)
        {
            if (!Success)
            {
                assembly = null;
                return this;
            }
#if NETCORE
            using var msa = new MemoryStream(Assembly!);
            using var mss = Symbols is not null ? new MemoryStream(Symbols) : null;
            assembly = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromStream(msa, mss);
#endif
#if NETSTANDARD || NETFRAMEWORK
            assembly = System.Reflection.Assembly.Load(Assembly, Symbols);
#endif
            return this;
        }

        /// <summary>
        /// Create a new file, writes the specified assembly byte array th the file
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public CompileResult TrySave(string directory)
        {
            if (Success)
            {
                File.WriteAllBytes(Path.Combine(directory, $"{AssemblyName}.dll"), Assembly!);

                if (Symbols is not null)
                    File.WriteAllBytes(Path.Combine(directory, $"{AssemblyName}.pdb"), Symbols);
            }

            return this;
        }
    }
}
