using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Steelax.EasyRoslyn.Helpers;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Steelax.EasyRoslyn.Common
{
    public class CompilationSettings
    {
        private CSharpCompilationOptions _cmpOptions;

        public string AssemblyName { get; private set; }
        internal CSharpCompilationOptions CompilationOptions => _cmpOptions;

        public CompilationSettings()
        {
            _cmpOptions = new CSharpCompilationOptions
            (
                outputKind: OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: OptimizationLevel.Release,
                checkOverflow: true,
                platform: GetDefaultPlatform()
            );
            AssemblyName = Path.GetRandomFileName();
        }
        private CompilationSettings(CompilationSettings other)
        {
            _cmpOptions = _cmpOptions.Clone();

            AssemblyName = other.AssemblyName;
        }
        private static Platform GetDefaultPlatform()
        {
            try
            {
                var arch = RuntimeInformation.ProcessArchitecture;

                if (arch == Architecture.Arm64)
                    return Platform.Arm64;
                if (arch == Architecture.Arm)
                    return Platform.Arm;
                if (arch == Architecture.X64)
                    return Platform.X64;
                if (arch == Architecture.X86)
                    return Platform.X86;

                return Platform.AnyCpu;
            }
            catch
            {
                return Platform.AnyCpu;
            }
        }

        public CompilationSettings WithAllowUnsafe(bool enabled)
        {
            _cmpOptions = _cmpOptions.WithAllowUnsafe(enabled);
            return new CompilationSettings(this);
        }
        public CompilationSettings WithGeneralDiagnosticOption(ReportDiagnostic value)
        {
            _cmpOptions = _cmpOptions.WithGeneralDiagnosticOption(value);
            return new CompilationSettings(this);
        }
        public CompilationSettings WithMetadataImportOptions(MetadataImportOptions value)
        {
            _cmpOptions = _cmpOptions.WithMetadataImportOptions(value);
            return new CompilationSettings(this);
        }
        public CompilationSettings WithNullableContextOptions(NullableContextOptions value)
        {
            _cmpOptions = _cmpOptions.WithNullableContextOptions(value);
            return new CompilationSettings(this);
        }
        public CompilationSettings WithPlatform(Platform value)
        {
            _cmpOptions = _cmpOptions.WithPlatform(value);
            return new CompilationSettings(this);
        }
        public CompilationSettings WithReportSuppressedDiagnostics(bool enabled)
        {
            _cmpOptions = _cmpOptions.WithReportSuppressedDiagnostics(enabled);
            return new CompilationSettings(this);
        }
        public CompilationSettings WithWarningLevel(int level)
        {
            _cmpOptions = _cmpOptions.WithWarningLevel(level);
            return new CompilationSettings(this);
        }
        public CompilationSettings WithAssemblyName(string assemblyName)
        {
            if (string.IsNullOrEmpty(assemblyName))
                throw new ArgumentNullException(nameof(assemblyName), "Assembly name can't be null or empty");

            AssemblyName = assemblyName;
            return new CompilationSettings(this);
        }
        public CompilationSettings WithRandomAssemblyName()
        {
            AssemblyName = Path.GetRandomFileName();
            return new CompilationSettings(this);
        }
    }
}
