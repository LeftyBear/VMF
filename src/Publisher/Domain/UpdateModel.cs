namespace Vmf.Publisher.Domain;

/// <summary>Identifies the lifecycle state of a published document.</summary>
public enum DocumentState
{
    /// <summary>The document has not been published.</summary>
    New,

    /// <summary>The destination document exists.</summary>
    Existing,

    /// <summary>The destination document is archived.</summary>
    Archived,

    /// <summary>The expected destination document is missing.</summary>
    Missing,
}

/// <summary>Identifies one document across a publication and Google Docs.</summary>
public sealed class DocumentIdentity
{
    /// <summary>Initializes a document identity.</summary>
    /// <param name="publicationId">The persistent publication identifier.</param>
    /// <param name="documentId">The publication-local document identifier.</param>
    /// <param name="googleDocumentId">The Google document identifier, when assigned.</param>
    /// <param name="state">The current document state.</param>
    public DocumentIdentity(
        string publicationId,
        string documentId,
        string? googleDocumentId,
        DocumentState state)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(publicationId);
        ArgumentException.ThrowIfNullOrWhiteSpace(documentId);
        if (googleDocumentId is not null && string.IsNullOrWhiteSpace(googleDocumentId))
        {
            throw new ArgumentException(
                "A Google document identifier must not be empty when supplied.",
                nameof(googleDocumentId));
        }

        PublicationId = publicationId;
        DocumentId = documentId;
        GoogleDocumentId = googleDocumentId;
        State = state;
    }

    /// <summary>Gets the persistent publication identifier.</summary>
    public string PublicationId { get; }

    /// <summary>Gets the publication-local document identifier.</summary>
    public string DocumentId { get; }

    /// <summary>Gets the Google document identifier, when assigned.</summary>
    public string? GoogleDocumentId { get; }

    /// <summary>Gets the current document state.</summary>
    public DocumentState State { get; }
}

/// <summary>Identifies a managed document block for differential publication.</summary>
public sealed class BlockIdentity
{
    /// <summary>Initializes a managed block identity.</summary>
    /// <param name="explicitId">The optional author-supplied identifier.</param>
    /// <param name="generatedId">The optional stable generated identifier.</param>
    /// <param name="contentHash">The required canonical content hash.</param>
    public BlockIdentity(string? explicitId, string? generatedId, string contentHash)
    {
        ValidateOptionalId(explicitId, nameof(explicitId));
        ValidateOptionalId(generatedId, nameof(generatedId));
        ArgumentException.ThrowIfNullOrWhiteSpace(contentHash);

        ExplicitId = explicitId;
        GeneratedId = generatedId;
        ContentHash = contentHash;
    }

    /// <summary>Gets the optional author-supplied identifier.</summary>
    public string? ExplicitId { get; }

    /// <summary>Gets the optional stable generated identifier.</summary>
    public string? GeneratedId { get; }

    /// <summary>Gets the canonical block content hash.</summary>
    public string ContentHash { get; }

    private static void ValidateOptionalId(string? value, string parameterName)
    {
        if (value is not null && string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                "A block identifier must not be empty when supplied.",
                parameterName);
        }
    }
}

/// <summary>Represents an opaque fingerprint of canonical publish input.</summary>
public sealed class PublishFingerprint : IEquatable<PublishFingerprint>
{
    /// <summary>Initializes a publish fingerprint.</summary>
    /// <param name="value">The canonical fingerprint value.</param>
    public PublishFingerprint(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    /// <summary>Gets the canonical fingerprint value.</summary>
    public string Value { get; }

    /// <inheritdoc />
    public bool Equals(PublishFingerprint? other) =>
        other is not null && string.Equals(Value, other.Value, StringComparison.Ordinal);

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as PublishFingerprint);

    /// <inheritdoc />
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

    /// <inheritdoc />
    public override string ToString() => Value;
}

/// <summary>Represents the verified state used as a differential publish baseline.</summary>
public sealed class PublishState
{
    /// <summary>Initializes a publish state.</summary>
    /// <param name="identity">The document identity.</param>
    /// <param name="fingerprint">The canonical publish fingerprint.</param>
    /// <param name="blocks">The managed blocks in publication order.</param>
    public PublishState(
        DocumentIdentity identity,
        PublishFingerprint fingerprint,
        IEnumerable<BlockIdentity> blocks)
    {
        Identity = identity ?? throw new ArgumentNullException(nameof(identity));
        Fingerprint = fingerprint ?? throw new ArgumentNullException(nameof(fingerprint));
        ArgumentNullException.ThrowIfNull(blocks);

        var items = blocks.ToArray();
        if (items.Any(item => item is null))
        {
            throw new ArgumentException("Publish state blocks must not contain null items.", nameof(blocks));
        }

        EnsureUnique(items, block => block.ExplicitId, "explicit", nameof(blocks));
        EnsureUnique(items, block => block.GeneratedId, "generated", nameof(blocks));
        Blocks = Array.AsReadOnly(items);
    }

    /// <summary>Gets the document identity.</summary>
    public DocumentIdentity Identity { get; }

    /// <summary>Gets the canonical publish fingerprint.</summary>
    public PublishFingerprint Fingerprint { get; }

    /// <summary>Gets the managed blocks in publication order.</summary>
    public IReadOnlyList<BlockIdentity> Blocks { get; }

    private static void EnsureUnique(
        IEnumerable<BlockIdentity> blocks,
        Func<BlockIdentity, string?> selector,
        string identifierKind,
        string parameterName)
    {
        var identifiers = new HashSet<string>(StringComparer.Ordinal);
        foreach (var identifier in blocks.Select(selector).Where(value => value is not null))
        {
            if (!identifiers.Add(identifier!))
            {
                throw new ArgumentException(
                    $"Publish state contains a duplicate {identifierKind} block identifier: {identifier}",
                    parameterName);
            }
        }
    }
}

/// <summary>Identifies a block-level differential operation.</summary>
public enum DiffOperationKind
{
    /// <summary>A new block must be inserted.</summary>
    Insert,

    /// <summary>An existing block's content must be updated.</summary>
    Update,

    /// <summary>An existing block must be deleted.</summary>
    Delete,

    /// <summary>An existing block must be moved.</summary>
    Move,

    /// <summary>The block requires no change.</summary>
    NoChange,
}

/// <summary>Identifies the block identity level that produced a match.</summary>
public enum BlockMatchKind
{
    /// <summary>The blocks matched by explicit identifier.</summary>
    ExplicitId,

    /// <summary>The blocks matched by generated identifier.</summary>
    GeneratedId,

    /// <summary>The blocks matched by content hash.</summary>
    ContentHash,
}

/// <summary>Describes one block-level differential operation.</summary>
public sealed class DiffOperation
{
    internal DiffOperation(
        DiffOperationKind kind,
        int? previousIndex,
        int? currentIndex,
        BlockIdentity? previousBlock,
        BlockIdentity? currentBlock,
        BlockMatchKind? matchKind)
    {
        Kind = kind;
        PreviousIndex = previousIndex;
        CurrentIndex = currentIndex;
        PreviousBlock = previousBlock;
        CurrentBlock = currentBlock;
        MatchKind = matchKind;
    }

    /// <summary>Gets the differential operation kind.</summary>
    public DiffOperationKind Kind { get; }

    /// <summary>Gets the zero-based prior block index, when applicable.</summary>
    public int? PreviousIndex { get; }

    /// <summary>Gets the zero-based desired block index, when applicable.</summary>
    public int? CurrentIndex { get; }

    /// <summary>Gets the prior block, when applicable.</summary>
    public BlockIdentity? PreviousBlock { get; }

    /// <summary>Gets the desired block, when applicable.</summary>
    public BlockIdentity? CurrentBlock { get; }

    /// <summary>Gets the identity level that produced the match, when applicable.</summary>
    public BlockMatchKind? MatchKind { get; }
}

/// <summary>Represents the block-level plan for a differential publication.</summary>
public sealed class DiffPlan
{
    internal DiffPlan(
        PublishFingerprint? previousFingerprint,
        PublishFingerprint currentFingerprint,
        bool isFingerprintMatch,
        IEnumerable<DiffOperation> operations)
    {
        PreviousFingerprint = previousFingerprint;
        CurrentFingerprint = currentFingerprint ?? throw new ArgumentNullException(nameof(currentFingerprint));
        IsFingerprintMatch = isFingerprintMatch;
        ArgumentNullException.ThrowIfNull(operations);
        Operations = Array.AsReadOnly(operations.ToArray());
    }

    /// <summary>Gets the prior fingerprint, when a baseline exists.</summary>
    public PublishFingerprint? PreviousFingerprint { get; }

    /// <summary>Gets the desired fingerprint.</summary>
    public PublishFingerprint CurrentFingerprint { get; }

    /// <summary>Gets a value indicating whether idempotency skipped block comparison.</summary>
    public bool IsFingerprintMatch { get; }

    /// <summary>Gets the block-level differential operations.</summary>
    public IReadOnlyList<DiffOperation> Operations { get; }

    /// <summary>Gets a value indicating whether the managed region requires a mutation.</summary>
    public bool IsPublishRequired =>
        !IsFingerprintMatch && Operations.Any(operation => operation.Kind != DiffOperationKind.NoChange);
}
