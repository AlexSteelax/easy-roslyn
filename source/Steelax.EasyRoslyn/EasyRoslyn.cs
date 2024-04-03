using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Steelax.EasyRoslyn.Abstractions;
using Steelax.EasyRoslyn.CodeAnalysis;

namespace Steelax.EasyRoslyn;

public static class EasyRoslyn
{
    public static IEasyRoslynBuilder<CSharpCompilationOptions, CSharpParseOptions> CreateCSharpBuilder(OutputKind outputKind) => new CSharpBuilder(outputKind);
}