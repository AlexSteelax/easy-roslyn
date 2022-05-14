using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;
using Xunit;
using static Steelax.EasyRoslyn.RoslynTasks;

namespace Steelax.EasyRoslyn.Tests
{
    public class RoslynTasksTest
    {
        [Fact]
        public void Compile()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(@"public class Test { public static int GetInt(int value) => value; }");
            var result = RoslynCompile(syntaxTree, references => references, compilation => compilation);

            Assert.True(result.Success);

            result.TryLoad(out Assembly assembly);

            Assert.NotNull(assembly);

            var type = assembly.GetType("Test", false);

            Assert.NotNull(type);

            var method = type.GetMethod("GetInt");

            Assert.NotNull(method);

            var value = method.Invoke(null, new object[] { 5 });

            Assert.Equal(5, value);
        }
    }
}