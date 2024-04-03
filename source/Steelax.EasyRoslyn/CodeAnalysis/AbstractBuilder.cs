using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using Steelax.EasyRoslyn.Abstractions;
using Steelax.EasyRoslyn.Collections;
using Steelax.EasyRoslyn.Common;
using System.IO;

namespace Steelax.EasyRoslyn.CodeAnalysis;

internal abstract partial class AbstractBuilder<TSelf, TCompilationOptions, TParseOptions> : IEasyRoslynBuilder<TCompilationOptions, TParseOptions>
    where TSelf: AbstractBuilder<TSelf, TCompilationOptions, TParseOptions>
    where TCompilationOptions : CompilationOptions
    where TParseOptions: ParseOptions
{
    private readonly ReferenceBag _referenceBag;
    private readonly SourceBag _sourceBag;
    private TCompilationOptions _compilationOptions;
    private TParseOptions _parseOptions;
    private EmitOptions _emitOptions;

    private string _assemblyName;

    internal AbstractBuilder(TCompilationOptions cmpOptions, TParseOptions parseOptions)
    {
        _compilationOptions = cmpOptions;
        _parseOptions = parseOptions;

        _referenceBag = new();
        _sourceBag = new();
        _emitOptions = new();

        _assemblyName = Path.GetRandomFileName();
    }

    protected abstract SyntaxTree Parse(SourceText source, string path);
    protected abstract Compilation CreateCompilation();

    public CompileResult Build()
    {
        var compilation = CreateCompilation();

        using var assemblyStream = new MemoryStream();
        using var symbolsStream = new MemoryStream();

        var result = compilation.Emit(
            peStream: assemblyStream,
            pdbStream: symbolsStream,
            embeddedTexts: EmbeddedTexts,
            options: _emitOptions);

        assemblyStream?.Seek(0, SeekOrigin.Begin);
        symbolsStream?.Seek(0, SeekOrigin.Begin);

        if (!result.Success)
        {
            return new CompileResult
            {
                Diagnostics = result.Diagnostics,
                Success = result.Success
            };
        }

        return new CompileResult
        {
            Assembly = assemblyStream?.ToArray(),
            Symbols = symbolsStream?.ToArray(),
            AssemblyName = AssemblyName,
            Diagnostics = result.Diagnostics,
            Success = result.Success
        };
    }
}
