using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace Steelax.EasyRoslyn.Common
{
    public class RoslynCompileResult
    {
        public byte[] Assembly { get; internal set; }
        public string AssemblyName { get; internal set; }
        public ImmutableArray<Diagnostic> Diagnostics { get; internal set; }
        public bool Success { get; internal set; }

        internal RoslynCompileResult() { }
    }
}
