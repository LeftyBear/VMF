using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Application;

/// <summary>Generates a versioned hash from canonical block content.</summary>
public interface IBlockContentHashGenerator
{
    /// <summary>Generates a content-only hash for one canonical document block.</summary>
    /// <param name="block">The canonical block payload.</param>
    /// <returns>The versioned content hash.</returns>
    string Generate(DocumentBlock block);
}
