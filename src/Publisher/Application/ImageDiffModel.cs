using System.Security.Cryptography;
using System.Text;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Application;

internal enum ImageDiffKind
{
    NoChange,
    Insert,
    Delete,
    Move,
    ReplaceContent,
    UpdatePresentation,
    MoveAndReplaceContent,
    MoveAndUpdatePresentation,
    ReplaceContentAndUpdatePresentation,
    MoveReplaceAndUpdatePresentation,
}

internal sealed class ImageDiffResult
{
    internal ImageDiffResult(
        ImageDiffKind kind,
        int? previousIndex,
        int? currentIndex,
        CanonicalImage? previousImage,
        CanonicalImage? currentImage,
        string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        Kind = kind;
        PreviousIndex = previousIndex;
        CurrentIndex = currentIndex;
        PreviousImage = previousImage;
        CurrentImage = currentImage;
        Reason = reason;
    }

    internal ImageDiffKind Kind { get; }

    internal int? PreviousIndex { get; }

    internal int? CurrentIndex { get; }

    internal CanonicalImage? PreviousImage { get; }

    internal CanonicalImage? CurrentImage { get; }

    internal string Reason { get; }
}

internal sealed class ImagePhysicalPlan
{
    internal ImagePhysicalPlan(IEnumerable<ImagePhysicalOperation> operations)
    {
        ArgumentNullException.ThrowIfNull(operations);
        Operations = Array.AsReadOnly(operations.ToArray());
    }

    internal IReadOnlyList<ImagePhysicalOperation> Operations { get; }
}

internal sealed record ImagePhysicalOperation(
    ImageDiffKind Kind,
    int? PreviousIndex,
    int? CurrentIndex,
    CanonicalImage? Image);

internal sealed class ImageOperationReceipt
{
    internal ImageOperationReceipt(
        string operationId,
        ImageDiffKind kind,
        string contentHash,
        bool updateApplied,
        bool verificationSucceeded)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(operationId);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentHash);
        OperationId = operationId;
        Kind = kind;
        ContentHash = contentHash;
        UpdateApplied = updateApplied;
        VerificationSucceeded = verificationSucceeded;
    }

    internal string OperationId { get; }

    internal ImageDiffKind Kind { get; }

    internal string ContentHash { get; }

    internal bool UpdateApplied { get; }

    internal bool VerificationSucceeded { get; }

    internal bool IsSuccessful => UpdateApplied && VerificationSucceeded;
}

internal interface IImageCanonicalizer
{
    CanonicalImage Create(DocumentBlock block, int blockIndex);
}

internal sealed class ImageCanonicalizer : IImageCanonicalizer
{
    public CanonicalImage Create(DocumentBlock block, int blockIndex)
    {
        ArgumentNullException.ThrowIfNull(block);
        if (block.Kind != DocumentBlockKind.Image || block.Image is null)
        {
            throw new PhysicalUpdateException(
                UpdateErrorCodes.PhysicalPlanInvalid,
                "Only image blocks can be canonicalized as images.");
        }

        var image = block.Image;
        var contentHash = image.ContentHash ?? HashParts(
            "image-content-fallback",
            [image.Source.Kind.ToString(), image.Source.SourceKey]);
        var stableId = image.StableId ?? block.ExplicitId;
        return new CanonicalImage(
            new ImageIdentity(stableId, contentHash, image.Source.SourceKey),
            image.Source,
            image.Presentation);
    }

    internal static string PresentationHash(ImagePresentation presentation) => HashParts(
        "image-presentation",
        [
            presentation.AltText,
            presentation.Size?.WidthPoints.ToString("R", System.Globalization.CultureInfo.InvariantCulture) ?? "",
            presentation.Size?.HeightPoints.ToString("R", System.Globalization.CultureInfo.InvariantCulture) ?? "",
        ]);

    private static string HashParts(string scope, IEnumerable<string> parts)
    {
        var payload = string.Join("\n", [scope, .. parts]);
        return ImageContentHash.ValuePrefix + Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(payload))).ToLowerInvariant();
    }
}
