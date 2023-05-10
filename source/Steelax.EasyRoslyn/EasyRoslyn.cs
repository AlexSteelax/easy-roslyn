using Microsoft.CodeAnalysis;
using Steelax.EasyRoslyn.CodeAnalysis;

namespace Steelax.EasyRoslyn
{
    public static class EasyRoslyn
    {
        public static CSharpBuilder CreateCSharpBuilder(OutputKind outputKind) => new CSharpBuilder(outputKind);
    }
}
