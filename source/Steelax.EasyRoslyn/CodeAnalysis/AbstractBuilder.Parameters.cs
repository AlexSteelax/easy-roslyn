using Microsoft.CodeAnalysis;
using Steelax.EasyRoslyn.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace Steelax.EasyRoslyn.CodeAnalysis;

internal abstract partial class AbstractBuilder<TSelf, TCompilationOptions, TParseOptions>
    where TSelf : AbstractBuilder<TSelf, TCompilationOptions, TParseOptions>
    where TCompilationOptions : CompilationOptions
    where TParseOptions : ParseOptions
{
    /// <summary>
    /// Get syntax trees from sources
    /// </summary>
    protected IEnumerable<SyntaxTree> SyntaxTrees
    {
        get
        {
            return _sourceBag.Select(o => Parse(o.SourceText, o.Path));
        }
    }

    /// <summary>
    /// Get embedded text (pdb) from sources
    /// </summary>
    protected IEnumerable<EmbeddedText> EmbeddedTexts
    {
        get
        {
            return _sourceBag.Select(o => EmbeddedText.FromSource(o.Path, o.SourceText));
        }
    }

    /// <summary>
    /// Get references
    /// </summary>
    protected IEnumerable<MetadataReference> References
    {
        get
        {
            yield return typeof(object).Assembly.GetMetadataReference();

            foreach (var assembly in _referenceBag)
                yield return assembly.GetMetadataReference();
        }
    }

    /// <summary>
    /// Get assembly name
    /// </summary>
    protected string AssemblyName
    {
        get
        {
            return _assemblyName;
        }
    }

    /// <summary>
    /// Get compilation options
    /// </summary>
    protected TCompilationOptions CompilationOptions
    {
        get
        {
            return _compilationOptions;
        }
    }

    /// <summary>
    /// Get source file parse options
    /// </summary>
    protected TParseOptions ParseOptions
    {
        get
        {
            return _parseOptions;
        }
    }
}
