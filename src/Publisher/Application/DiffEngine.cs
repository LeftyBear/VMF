using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Application;

/// <summary>Matches managed blocks by v1.0 identity precedence and calculates their differences.</summary>
public sealed class DiffEngine : IDiffEngine
{
    /// <inheritdoc />
    public DiffPlan CreatePlan(PublishState? previousState, PublishState currentState)
    {
        ArgumentNullException.ThrowIfNull(currentState);

        if (previousState is null)
        {
            return new DiffPlan(
                null,
                currentState.Fingerprint,
                false,
                currentState.Blocks.Select((block, index) => new DiffOperation(
                    DiffOperationKind.Insert,
                    null,
                    index,
                    null,
                    block,
                    null)));
        }

        EnsureSameDocument(previousState.Identity, currentState.Identity);
        if (previousState.Fingerprint.Equals(currentState.Fingerprint))
        {
            return new DiffPlan(
                previousState.Fingerprint,
                currentState.Fingerprint,
                true,
                Array.Empty<DiffOperation>());
        }

        var matches = MatchBlocks(previousState.Blocks, currentState.Blocks);
        var operations = BuildOperations(previousState.Blocks, currentState.Blocks, matches);
        return new DiffPlan(
            previousState.Fingerprint,
            currentState.Fingerprint,
            false,
            operations);
    }

    private static void EnsureSameDocument(DocumentIdentity previous, DocumentIdentity current)
    {
        if (!string.Equals(previous.PublicationId, current.PublicationId, StringComparison.Ordinal) ||
            !string.Equals(previous.DocumentId, current.DocumentId, StringComparison.Ordinal) ||
            (previous.GoogleDocumentId is not null &&
             current.GoogleDocumentId is not null &&
             !string.Equals(
                 previous.GoogleDocumentId,
                 current.GoogleDocumentId,
                 StringComparison.Ordinal)))
        {
            throw new DiffConflictException(
                "DIFF_DOCUMENT_IDENTITY_MISMATCH",
                "The baseline and desired publish states do not identify the same document.");
        }
    }

    private static MatchResult[] MatchBlocks(
        IReadOnlyList<BlockIdentity> previous,
        IReadOnlyList<BlockIdentity> current)
    {
        var matches = new MatchResult[current.Count];
        var matchedPrevious = new bool[previous.Count];

        MatchUniqueIdentifiers(
            previous,
            current,
            block => block.ExplicitId,
            BlockMatchKind.ExplicitId,
            matches,
            matchedPrevious);
        MatchUniqueIdentifiers(
            previous,
            current,
            block => block.GeneratedId,
            BlockMatchKind.GeneratedId,
            matches,
            matchedPrevious);
        MatchContentHashes(previous, current, matches, matchedPrevious);

        return matches;
    }

    private static void MatchUniqueIdentifiers(
        IReadOnlyList<BlockIdentity> previous,
        IReadOnlyList<BlockIdentity> current,
        Func<BlockIdentity, string?> selector,
        BlockMatchKind matchKind,
        MatchResult[] matches,
        bool[] matchedPrevious)
    {
        var previousByIdentifier = new Dictionary<string, int>(StringComparer.Ordinal);
        for (var index = 0; index < previous.Count; index++)
        {
            if (matchedPrevious[index])
            {
                continue;
            }

            var identifier = selector(previous[index]);
            if (identifier is not null)
            {
                previousByIdentifier.Add(identifier, index);
            }
        }

        for (var index = 0; index < current.Count; index++)
        {
            if (matches[index].IsMatched)
            {
                continue;
            }

            var identifier = selector(current[index]);
            if (identifier is null || !previousByIdentifier.TryGetValue(identifier, out var previousIndex))
            {
                continue;
            }

            matches[index] = new MatchResult(previousIndex, matchKind);
            matchedPrevious[previousIndex] = true;
        }
    }

    private static void MatchContentHashes(
        IReadOnlyList<BlockIdentity> previous,
        IReadOnlyList<BlockIdentity> current,
        MatchResult[] matches,
        bool[] matchedPrevious)
    {
        var previousByHash = new Dictionary<string, Queue<int>>(StringComparer.Ordinal);
        for (var index = 0; index < previous.Count; index++)
        {
            if (matchedPrevious[index])
            {
                continue;
            }

            if (!previousByHash.TryGetValue(previous[index].ContentHash, out var indexes))
            {
                indexes = new Queue<int>();
                previousByHash.Add(previous[index].ContentHash, indexes);
            }

            indexes.Enqueue(index);
        }

        for (var index = 0; index < current.Count; index++)
        {
            if (matches[index].IsMatched ||
                !previousByHash.TryGetValue(current[index].ContentHash, out var indexes) ||
                indexes.Count == 0)
            {
                continue;
            }

            var previousIndex = indexes.Dequeue();
            matches[index] = new MatchResult(previousIndex, BlockMatchKind.ContentHash);
            matchedPrevious[previousIndex] = true;
        }
    }

    private static IReadOnlyList<DiffOperation> BuildOperations(
        IReadOnlyList<BlockIdentity> previous,
        IReadOnlyList<BlockIdentity> current,
        MatchResult[] matches)
    {
        var operations = new List<DiffOperation>();
        var matchedPrevious = new bool[previous.Count];
        var movedCurrent = FindMovedCurrentBlocks(matches);

        for (var currentIndex = 0; currentIndex < current.Count; currentIndex++)
        {
            var match = matches[currentIndex];
            if (!match.IsMatched)
            {
                operations.Add(new DiffOperation(
                    DiffOperationKind.Insert,
                    null,
                    currentIndex,
                    null,
                    current[currentIndex],
                    null));
                continue;
            }

            var previousIndex = match.PreviousIndex;
            matchedPrevious[previousIndex] = true;
            var contentChanged = !string.Equals(
                previous[previousIndex].ContentHash,
                current[currentIndex].ContentHash,
                StringComparison.Ordinal);

            if (movedCurrent[currentIndex])
            {
                operations.Add(new DiffOperation(
                    DiffOperationKind.Move,
                    previousIndex,
                    currentIndex,
                    previous[previousIndex],
                    current[currentIndex],
                    match.Kind));
            }

            if (contentChanged)
            {
                operations.Add(new DiffOperation(
                    DiffOperationKind.Update,
                    previousIndex,
                    currentIndex,
                    previous[previousIndex],
                    current[currentIndex],
                    match.Kind));
            }

            if (!movedCurrent[currentIndex] && !contentChanged)
            {
                operations.Add(new DiffOperation(
                    DiffOperationKind.NoChange,
                    previousIndex,
                    currentIndex,
                    previous[previousIndex],
                    current[currentIndex],
                    match.Kind));
            }
        }

        for (var previousIndex = 0; previousIndex < previous.Count; previousIndex++)
        {
            if (!matchedPrevious[previousIndex])
            {
                operations.Add(new DiffOperation(
                    DiffOperationKind.Delete,
                    previousIndex,
                    null,
                    previous[previousIndex],
                    null,
                    null));
            }
        }

        return operations;
    }

    private static bool[] FindMovedCurrentBlocks(MatchResult[] matches)
    {
        var matchedCurrentIndexes = Enumerable.Range(0, matches.Length)
            .Where(index => matches[index].IsMatched)
            .ToArray();
        var movedCurrent = new bool[matches.Length];
        if (matchedCurrentIndexes.Length == 0)
        {
            return movedCurrent;
        }

        var tailSequenceIndexes = new int[matchedCurrentIndexes.Length];
        var predecessorSequenceIndexes = Enumerable.Repeat(-1, matchedCurrentIndexes.Length).ToArray();
        var longestLength = 0;

        for (var sequenceIndex = 0; sequenceIndex < matchedCurrentIndexes.Length; sequenceIndex++)
        {
            var previousIndex = matches[matchedCurrentIndexes[sequenceIndex]].PreviousIndex;
            var lower = 0;
            var upper = longestLength;
            while (lower < upper)
            {
                var middle = lower + ((upper - lower) / 2);
                var tailPreviousIndex = matches[
                    matchedCurrentIndexes[tailSequenceIndexes[middle]]].PreviousIndex;
                if (tailPreviousIndex < previousIndex)
                {
                    lower = middle + 1;
                }
                else
                {
                    upper = middle;
                }
            }

            if (lower > 0)
            {
                predecessorSequenceIndexes[sequenceIndex] = tailSequenceIndexes[lower - 1];
            }

            tailSequenceIndexes[lower] = sequenceIndex;
            if (lower == longestLength)
            {
                longestLength++;
            }
        }

        var stableSequenceIndexes = new bool[matchedCurrentIndexes.Length];
        for (var sequenceIndex = tailSequenceIndexes[longestLength - 1];
             sequenceIndex >= 0;
             sequenceIndex = predecessorSequenceIndexes[sequenceIndex])
        {
            stableSequenceIndexes[sequenceIndex] = true;
        }

        for (var sequenceIndex = 0; sequenceIndex < matchedCurrentIndexes.Length; sequenceIndex++)
        {
            if (!stableSequenceIndexes[sequenceIndex])
            {
                movedCurrent[matchedCurrentIndexes[sequenceIndex]] = true;
            }
        }

        return movedCurrent;
    }

    private readonly struct MatchResult
    {
        internal MatchResult(int previousIndex, BlockMatchKind kind)
        {
            PreviousIndex = previousIndex;
            Kind = kind;
            IsMatched = true;
        }

        internal int PreviousIndex { get; }

        internal BlockMatchKind Kind { get; }

        internal bool IsMatched { get; }
    }
}
