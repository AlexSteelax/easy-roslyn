using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Runtime.InteropServices;

namespace Steelax.EasyRoslyn;

public static class CompileOptionsExtension
{
    public static CSharpCompilationOptions WithCurrentPlatform(this CSharpCompilationOptions options)
    {
        return options.WithPlatform(GetCurrentPlatform());
    }

    private static Platform GetCurrentPlatform() => RuntimeInformation.ProcessArchitecture switch
    {
        Architecture.Arm64 => Platform.Arm64,
        Architecture.Arm => Platform.Arm,
        Architecture.X64 => Platform.X64,
        Architecture.X86 => Platform.X86,
        _ => Platform.AnyCpu
    };
}
