using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Application;

internal sealed class ImageDiffEngine
{
    private readonly IImageCanonicalizer canonicalizer;

    internal ImageDiffEngine()
        : this(new ImageCanonicalizer())
    {
    }

    internal ImageDiffEngine(IImageCanonicalizer canonicalizer)
    {
        this.canonicalizer = canonicalizer ?? throw new ArgumentNullException(nameof(canonicalizer));
    }

    internal IReadOnlyList<ImageDiffResult> CreatePlan(
        IReadOnlyList<DocumentBlock> previousBlocks,
        IReadOnlyList<DocumentBlock> currentBlocks)
    {
        ArgumentNullException.ThrowIfNull(previousBlocks);
        ArgumentNullException.ThrowIfNull(currentBlocks);
        var previous = Images(previousBlocks);
        var current = Images(currentBlocks);
        var matches = Match(previousBlocks, currentBlocks, previous, current);
        var matchedPrevious = new bool[previous.Count];
        var results = new List<ImageDiffResult>();

        for (var currentIndex = 0; currentIndex < current.Count; currentIndex++)
        {
            if (matches[currentIndex] is not int previousIndex)
            {
                results.Add(new ImageDiffResult(
                    ImageDiffKind.Insert,
                    null,
                    currentIndex,
                    null,
                    current[currentIndex].Image,
                    "image missing from baseline"));
                continue;
            }

            matchedPrevious[previousIndex] = true;
            var moved = previous[previousIndex].BlockIndex != current[currentIndex].BlockIndex;
            var contentChanged = !string.Equals(
                previous[previousIndex].Image.Identity.ContentHash,
                current[currentIndex].Image.Identity.ContentHash,
                StringComparison.Ordinal);
            var presentationChanged = !string.Equals(
                ImageCanonicalizer.PresentationHash(previous[previousIndex].Image.Presentation),
                ImageCanonicalizer.PresentationHash(current[currentIndex].Image.Presentation),
                StringComparison.Ordinal);

            results.Add(new ImageDiffResult(
                Kind(moved, contentChanged, presentationChanged),
                previousIndex,
                currentIndex,
                previous[previousIndex].Image,
                current[currentIndex].Image,
                "image classified by identity precedence"));
        }

        for (var previousIndex = 0; previousIndex < previous.Count; previousIndex++)
        {
            if (!matchedPrevious[previousIndex])
            {
                results.Add(new ImageDiffResult(
                    ImageDiffKind.Delete,
                    previousIndex,
                    null,
                    previous[previousIndex].Image,
                    null,
                    "image missing from candidate"));
            }
        }

        return Array.AsReadOnly(results.OrderBy(Signature).ToArray());
    }

    internal ImagePhysicalPlan CreatePhysicalPlan(IEnumerable<ImageDiffResult> imageDiff)
    {
        ArgumentNullException.ThrowIfNull(imageDiff);
        var operations = imageDiff
            .SelectMany(Expand)
            .OrderBy(operation => operation.Kind == ImageDiffKind.Delete ? 0 : 1)
            .ThenByDescending(operation => operation.Kind == ImageDiffKind.Delete
                ? operation.PreviousIndex
                : operation.CurrentIndex)
            .ToArray();
        return new ImagePhysicalPlan(operations);
    }

    private static IEnumerable<ImagePhysicalOperation> Expand(ImageDiffResult result)
    {
        if (result.Kind == ImageDiffKind.NoChange)
        {
            yield break;
        }

        if (result.Kind == ImageDiffKind.Delete)
        {
            yield return new ImagePhysicalOperation(
                ImageDiffKind.Delete,
                result.PreviousIndex,
                null,
                result.PreviousImage);
            yield break;
        }

        if (result.Kind == ImageDiffKind.Insert)
        {
            yield return new ImagePhysicalOperation(
                ImageDiffKind.Insert,
                null,
                result.CurrentIndex,
                result.CurrentImage);
            yield break;
        }

        yield return new ImagePhysicalOperation(
            ImageDiffKind.Delete,
            result.PreviousIndex,
            null,
            result.PreviousImage);
        yield return new ImagePhysicalOperation(
            ImageDiffKind.Insert,
            null,
            result.CurrentIndex,
            result.CurrentImage);
    }

    private static ImageDiffKind Kind(bool moved, bool contentChanged, bool presentationChanged) =>
        (moved, contentChanged, presentationChanged) switch
        {
            (false, false, false) => ImageDiffKind.NoChange,
            (true, false, false) => ImageDiffKind.Move,
            (false, true, false) => ImageDiffKind.ReplaceContent,
            (false, false, true) => ImageDiffKind.UpdatePresentation,
            (true, true, false) => ImageDiffKind.MoveAndReplaceContent,
            (true, false, true) => ImageDiffKind.MoveAndUpdatePresentation,
            (false, true, true) => ImageDiffKind.ReplaceContentAndUpdatePresentation,
            (true, true, true) => ImageDiffKind.MoveReplaceAndUpdatePresentation,
        };

    private IReadOnlyList<IndexedImage> Images(IReadOnlyList<DocumentBlock> blocks) =>
        blocks
            .Select((block, index) => (Block: block, Index: index))
            .Where(item => item.Block.Kind == DocumentBlockKind.Image)
            .Select((item, imageIndex) => new IndexedImage(
                imageIndex,
                item.Index,
                canonicalizer.Create(item.Block, item.Index)))
            .ToArray();

    private static int?[] Match(
        IReadOnlyList<DocumentBlock> previousBlocks,
        IReadOnlyList<DocumentBlock> currentBlocks,
        IReadOnlyList<IndexedImage> previous,
        IReadOnlyList<IndexedImage> current)
    {
        var matches = new int?[current.Count];
        var matchedPrevious = new bool[previous.Count];
        MatchTier(current, previous, matches, matchedPrevious, image => image.Identity.StableId);
        MatchTier(current, previous, matches, matchedPrevious, image => image.Identity.ContentHash);
        MatchTier(current, previous, matches, matchedPrevious, image => image.Identity.SourceKey);
        MatchSurroundingBlocks(previousBlocks, currentBlocks, previous, current, matches, matchedPrevious);
        return matches;
    }

    private static void MatchTier(
        IReadOnlyList<IndexedImage> current,
        IReadOnlyList<IndexedImage> previous,
        int?[] matches,
        bool[] matchedPrevious,
        Func<CanonicalImage, string?> selector)
    {
        var previousGroups = previous
            .Select((item, index) => (Key: selector(item.Image), Index: index))
            .Where(item => !string.IsNullOrWhiteSpace(item.Key) && !matchedPrevious[item.Index])
            .GroupBy(item => item.Key!, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.Select(item => item.Index).ToArray(), StringComparer.Ordinal);
        var currentGroups = current
            .Select((item, index) => (Key: selector(item.Image), Index: index))
            .Where(item => !string.IsNullOrWhiteSpace(item.Key) && matches[item.Index] is null)
            .GroupBy(item => item.Key!, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.Select(item => item.Index).ToArray(), StringComparer.Ordinal);

        foreach (var group in currentGroups)
        {
            if (!previousGroups.TryGetValue(group.Key, out var previousIndexes) ||
                group.Value.Length != 1 ||
                previousIndexes.Length != 1)
            {
                continue;
            }

            matches[group.Value[0]] = previousIndexes[0];
            matchedPrevious[previousIndexes[0]] = true;
        }
    }

    private static void MatchSurroundingBlocks(
        IReadOnlyList<DocumentBlock> previousBlocks,
        IReadOnlyList<DocumentBlock> currentBlocks,
        IReadOnlyList<IndexedImage> previous,
        IReadOnlyList<IndexedImage> current,
        int?[] matches,
        bool[] matchedPrevious)
    {
        for (var currentIndex = 0; currentIndex < current.Count; currentIndex++)
        {
            if (matches[currentIndex] is not null)
            {
                continue;
            }

            var candidates = Enumerable.Range(0, previous.Count)
                .Where(index => !matchedPrevious[index] &&
                    previous[index].BlockIndex == current[currentIndex].BlockIndex &&
                    SurroundingSignature(previousBlocks, previous[index].BlockIndex) ==
                    SurroundingSignature(currentBlocks, current[currentIndex].BlockIndex))
                .ToArray();
            if (candidates.Length != 1)
            {
                continue;
            }

            var previousIndex = candidates[0];
            matches[currentIndex] = previousIndex;
            matchedPrevious[previousIndex] = true;
        }
    }

    private static string SurroundingSignature(IReadOnlyList<DocumentBlock> blocks, int index) => string.Join(
        ":",
        index > 0 ? BlockSignature(blocks[index - 1]) : "",
        index + 1 < blocks.Count ? BlockSignature(blocks[index + 1]) : "");

    private static string BlockSignature(DocumentBlock block) =>
        $"{block.Kind}:{block.ExplicitId}:{block.Image?.ContentHash}:{block.Image?.Source.SourceKey}";

    private static string Signature(ImageDiffResult result) => string.Join(
        ":",
        result.Kind,
        result.PreviousIndex?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "-",
        result.CurrentIndex?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "-");

    private sealed record IndexedImage(int ImageIndex, int BlockIndex, CanonicalImage Image);
}
