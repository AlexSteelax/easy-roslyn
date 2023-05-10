using Microsoft.CodeAnalysis;
using System;
using System.Reflection;
using System.Reflection.Metadata;

namespace Steelax.EasyRoslyn.Helpers
{
    internal static class Extension
    {
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
