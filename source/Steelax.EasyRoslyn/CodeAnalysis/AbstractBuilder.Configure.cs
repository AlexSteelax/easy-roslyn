using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Steelax.EasyRoslyn.Abstractions;
using Steelax.EasyRoslyn.Collections;
using System;

namespace Steelax.EasyRoslyn.CodeAnalysis;

internal abstract partial class AbstractBuilder<TSelf, TCompilationOptions, TParseOptions>
    where TSelf : AbstractBuilder<TSelf, TCompilationOptions, TParseOptions>
    where TCompilationOptions : CompilationOptions
    where TParseOptions : ParseOptions
{
    public IEasyRoslynBuilder<TCompilationOptions, TParseOptions> ConfigureEmitOptions(Func<EmitOptions, EmitOptions> configure)
    {
        _emitOptions = configure(_emitOptions);
        return this;
    }
    public IEasyRoslynBuilder<TCompilationOptions, TParseOptions> ConfigureCompilationOptions(Func<TCompilationOptions, TCompilationOptions> configure)
    {
        _compilationOptions = configure(_compilationOptions);
        return this;
    }
    public IEasyRoslynBuilder<TCompilationOptions, TParseOptions> ConfigureParseOptions(Func<TParseOptions, TParseOptions> configure)
    {
        _parseOptions = configure(_parseOptions);
        return this;
    }
    public IEasyRoslynBuilder<TCompilationOptions, TParseOptions> ConfigureSources(Action<SourceBag> configure)
    {
        configure(_sourceBag);
        return this;
    }
    public IEasyRoslynBuilder<TCompilationOptions, TParseOptions> ConfigureReferences(Action<ReferenceBag> configure)
    {
        configure(_referenceBag);
        return this;
    }
    public IEasyRoslynBuilder<TCompilationOptions, TParseOptions> SetAssemblyName(string name)
    {
        _assemblyName = name;
        return this;
    }
}
