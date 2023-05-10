using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Steelax.EasyRoslyn.CodeAnalysis
{
    public sealed class CSharpBuilder : CodeBuilder<CSharpBuilder, CSharpCompilationOptions, CSharpParseOptions>
    {
        internal CSharpBuilder(OutputKind outputKind): base(new(outputKind), new())
        {
        }

        protected override Compilation CreateCompilation() => CSharpCompilation.Create(AssemblyName, SyntaxTrees, References, CompilationOptions);
        protected override SyntaxTree Parse(SourceText source, string path) => CSharpSyntaxTree.ParseText(source, ParseOptions, path);
    }
}
