using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Reflection;
using System.Reflection.Metadata;

namespace Steelax.EasyRoslyn.Helpers
{
    internal static class Extension
    {
        private readonly static ConstructorInfo CSharpCompilationOptionsConstructor =
            typeof(CSharpCompilationOptions).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(CSharpCompilationOptions) }, null);

        internal static CSharpCompilationOptions Clone(this CSharpCompilationOptions other) =>
            (CSharpCompilationOptions)CSharpCompilationOptionsConstructor.Invoke(new object[] { other });

        internal static MetadataReference GetMetadataReference(this Assembly assembly)
        {
#if NETCORE
            unsafe
            {
                assembly.TryGetRawMetadata(out byte* blob, out int length);
                return AssemblyMetadata.Create(ModuleMetadata.CreateFromMetadata((IntPtr)blob, length)).GetReference();
            }
#endif
#if NETSTANDARD
            return AssemblyMetadata.CreateFromFile(assembly.Location).GetReference();
#endif
#if NETFRAMEWORK
            return AssemblyMetadata.CreateFromFile(assembly.Location).GetReference();
#endif
        }
    }
}
