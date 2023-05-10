//using static Steelax.EasyRoslyn.RoslynTasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Steelax.EasyRoslyn.Common;
using System.Reflection;
using System.Text;
using Xunit;

namespace Steelax.EasyRoslyn.Tests
{
    public class RoslynTasksTest
    {
        static string CompileResultLog(CompileResult result)
        {
            var builder = new StringBuilder();

            var line = new string('=', 40);

            foreach (var diagnostic in result.Diagnostics)
            {
                builder.AppendLine($"{diagnostic.Severity} {diagnostic.Id} : {diagnostic.GetMessage()}");
            }

            builder.AppendLine(line);

            return builder.ToString();
        }

        [Fact]
        public void Compile()
        {
            var code = @"public class Test { public static int GetInt(int value) => value; }";

            var result = EasyRoslyn
                .CreateCSharpBuilder(OutputKind.DynamicallyLinkedLibrary)
                .ConfigureCompilationOptions(s => s.WithCurrentPlatform())
                .ConfigureSources(s => s.FromText(code, "Class.cs"))
                .ConfigureEmitOptions(s => s.WithDebugInformationFormat(DebugInformationFormat.PortablePdb))
                .Build();

            Assert.True(result.Success, CompileResultLog(result));

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