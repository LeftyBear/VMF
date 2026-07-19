using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Application;

/// <summary>Compiles a document model into publisher-neutral document operations.</summary>
public interface IDocumentCompiler
{
    /// <summary>Compiles a document model.</summary>
    /// <param name="document">The source document model.</param>
    /// <param name="title">The destination document title.</param>
    /// <returns>The compiled document.</returns>
    CompiledDocument Compile(DocumentModel document, string title);
}
