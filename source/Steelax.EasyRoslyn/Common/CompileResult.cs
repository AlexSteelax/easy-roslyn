using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.IO;
using System.Reflection;
using System.Text;

namespace Steelax.EasyRoslyn.Common
{
    public class CompileResult
    {
        public byte[] Assembly { get; internal set; }
        public byte[] Symbols { get; internal set; }
        public string AssemblyName { get; internal set; }
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
        /// Load the assembly into the application domain
        /// </summary>
        /// <param name="assembly">The loaded assembly</param>
        /// <returns></returns>
        public CompileResult TryLoad(out Assembly assembly)
        {
            assembly = Success ? System.Reflection.Assembly.Load(Assembly, Symbols) : null;

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
                File.WriteAllBytes(Path.Combine(directory, $"{AssemblyName}.dll"), Assembly);

                if (Symbols is not null)
                    File.WriteAllBytes(Path.Combine(directory, $"{AssemblyName}.pdb"), Symbols);
            }

            return this;
        }
    }
}
