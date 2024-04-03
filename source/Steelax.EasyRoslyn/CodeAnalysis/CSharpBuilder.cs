using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using Steelax.EasyRoslyn.Helpers;

namespace Steelax.EasyRoslyn.CodeAnalysis;

internal sealed class CSharpBuilder : AbstractBuilder<CSharpBuilder, CSharpCompilationOptions, CSharpParseOptions>
{
    internal CSharpBuilder(OutputKind outputKind): base(new(outputKind), new())
    {
        var isDebug = false;
        Extensions.IsDebugCheck(ref isDebug);

        ConfigureCompilationOptions(s => s.WithPlatform(Platform.AnyCpu).WithOptimizationLevel(isDebug ? OptimizationLevel.Debug : OptimizationLevel.Release));
        ConfigureEmitOptions(s => s.WithDebugInformationFormat(DebugInformationFormat.PortablePdb));
    }

    protected override Compilation CreateCompilation() => CSharpCompilation.Create(AssemblyName, SyntaxTrees, References, CompilationOptions);
    protected override SyntaxTree Parse(SourceText source, string path) => CSharpSyntaxTree.ParseText(source, ParseOptions, path);
}
