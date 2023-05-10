using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Steelax.EasyRoslyn
{
    public static class CompileOptionsExtension
    {
        public static CSharpCompilationOptions WithCurrentPlatform(this CSharpCompilationOptions options)
        {
            return options.WithPlatform(GetCurrentPlatform());
        }

        public static Platform GetCurrentPlatform() => RuntimeInformation.ProcessArchitecture switch
        {
            Architecture.Arm64 => Platform.Arm64,
            Architecture.Arm => Platform.Arm,
            Architecture.X64 => Platform.X64,
            Architecture.X86 => Platform.X86,
            _ => Platform.AnyCpu
        };
    }
}
