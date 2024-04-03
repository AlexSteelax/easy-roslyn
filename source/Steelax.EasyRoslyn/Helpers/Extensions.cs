using Microsoft.CodeAnalysis;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;

namespace Steelax.EasyRoslyn.Helpers;

internal static class Extensions
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
#if NETSTANDARD || NETFRAMEWORK
            return AssemblyMetadata.CreateFromFile(assembly.Location).GetReference();
#endif
    }

    [Conditional("DEBUG")]
    internal static void IsDebugCheck(ref bool isDebug) => isDebug = true;
}
