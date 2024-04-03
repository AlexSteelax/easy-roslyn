using Microsoft.CodeAnalysis.Emit;
using Steelax.EasyRoslyn.Collections;
using Steelax.EasyRoslyn.Common;
using System;

namespace Steelax.EasyRoslyn.Abstractions;

public interface IEasyRoslynBuilder<TCompilationOptions, TParseOptions>
{
    /// <summary>
    /// Configure emit options
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    IEasyRoslynBuilder<TCompilationOptions, TParseOptions> ConfigureEmitOptions(Func<EmitOptions, EmitOptions> configure);

    /// <summary>
    /// Configure compilation options
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    IEasyRoslynBuilder<TCompilationOptions, TParseOptions> ConfigureCompilationOptions(Func<TCompilationOptions, TCompilationOptions> configure);

    /// <summary>
    /// Configure file parse options
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    IEasyRoslynBuilder<TCompilationOptions, TParseOptions> ConfigureParseOptions(Func<TParseOptions, TParseOptions> configure);

    /// <summary>
    /// Configure code sources
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    IEasyRoslynBuilder<TCompilationOptions, TParseOptions> ConfigureSources(Action<SourceBag> configure);

    /// <summary>
    /// Configure used references
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    IEasyRoslynBuilder<TCompilationOptions, TParseOptions> ConfigureReferences(Action<ReferenceBag> configure);

    /// <summary>
    /// Define assembly name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IEasyRoslynBuilder<TCompilationOptions, TParseOptions> SetAssemblyName(string name);

    /// <summary>
    /// Parse source, compile and build assembly
    /// </summary>
    /// <returns></returns>
    CompileResult Build();
}