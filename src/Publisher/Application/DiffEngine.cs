using Vmf.Publisher.Domain;

namespace Vmf.Publisher.Application;

/// <summary>Matches managed blocks by v1.0 identity precedence and calculates their differences.</summary>
public sealed class DiffEngine : IDiffEngine
{
    /// <inheritdoc />
    public DiffPlan CreatePlan(VerifiedPublishState? baseline, PublishCandidate candidate)
    {
        ArgumentNullException.ThrowIfNull(candidate);
        EnsureUniqueIdentities(candidate.Blocks, "candidate");

        if (baseline is null)
        {
            return new DiffPlan(
                null,
                candidate.Fingerprint,
                false,
                candidate.Blocks.Select((block, index) => new DiffOperation(
                    DiffOperationKind.Insert,
                    null,
                    index,
                    null,
                    block,
                    null)));
        }

        EnsureUniqueIdentities(baseline.Blocks, "baseline");
        EnsureSameDocument(baseline.Identity, candidate.Identity);
        var matches = MatchBlocks(baseline.Blocks, candidate.Blocks);

        if (baseline.Fingerprint.Equals(candidate.Fingerprint))
        {
            return new DiffPlan(
                baseline.Fingerprint,
                candidate.Fingerprint,
                true,
                Array.Empty<DiffOperation>());
        }

        var operations = BuildOperations(baseline.Blocks, candidate.Blocks, matches);
        return new DiffPlan(
            baseline.Fingerprint,
            candidate.Fingerprint,
            false,
            operations);
    }

    private static void EnsureUniqueIdentities(
        IReadOnlyList<BlockIdentity> blocks,
        string stateRole)
    {
        EnsureUniqueIdentityTier(blocks, block => block.ExplicitId, "ExplicitId", stateRole);
        EnsureUniqueIdentityTier(blocks, block => block.GeneratedId, "GeneratedId", stateRole);
    }

    private static void EnsureUniqueIdentityTier(
        IReadOnlyList<BlockIdentity> blocks,
        Func<BlockIdentity, string?> selector,
        string identityName,
        string stateRole)
    {
        var indexes = new Dictionary<string, int>(StringComparer.Ordinal);
        for (var index = 0; index < blocks.Count; index++)
        {
            var identity = selector(blocks[index]);
            if (identity is null)
            {
                continue;
            }

            if (!indexes.TryAdd(identity, index))
            {
                throw new DiffConflictException(
                    DiffErrorCodes.DuplicateIdentity,
                    $"The {stateRole} contains a duplicate {identityName}: {identity}");
            }
        }
    }

    private static void EnsureSameDocument(DocumentIdentity baseline, DocumentIdentity candidate)
    {
        var googleDocumentIdMatches =
            (baseline.GoogleDocumentId is null && candidate.GoogleDocumentId is null) ||
            (baseline.GoogleDocumentId is not null &&
             candidate.GoogleDocumentId is not null &&
             string.Equals(
                 baseline.GoogleDocumentId,
                 candidate.GoogleDocumentId,
                 StringComparison.Ordinal));

        if (!string.Equals(baseline.PublicationId, candidate.PublicationId, StringComparison.Ordinal) ||
            !string.Equals(baseline.DocumentId, candidate.DocumentId, StringComparison.Ordinal) ||
            !googleDocumentIdMatches)
        {
            throw new DiffConflictException(
                DiffErrorCodes.DocumentIdentityMismatch,
                "The baseline and candidate do not identify the same document.");
        }
    }

    private static MatchResult[] MatchBlocks(
        IReadOnlyList<BlockIdentity> baseline,
        IReadOnlyList<BlockIdentity> candidate)
    {
        var matches = new MatchResult[candidate.Count];
        var matchedBaseline = new bool[baseline.Count];
        var baselineByExplicitId = CreateIdentityMap(baseline, block => block.ExplicitId);
        var baselineByGeneratedId = CreateIdentityMap(baseline, block => block.GeneratedId);
        var baselineByContentHash = CreateContentHashMap(baseline);

        MatchStrongIdentities(
            baseline,
            candidate,
            baselineByExplicitId,
            baselineByGeneratedId,
            baselineByContentHash,
            matches,
            matchedBaseline);
        MatchUnambiguousContentHashes(baseline, candidate, matches, matchedBaseline);

        return matches;
    }

    private static Dictionary<string, int> CreateIdentityMap(
        IReadOnlyList<BlockIdentity> blocks,
        Func<BlockIdentity, string?> selector)
    {
        var result = new Dictionary<string, int>(StringComparer.Ordinal);
        for (var index = 0; index < blocks.Count; index++)
        {
            var identity = selector(blocks[index]);
            if (identity is not null)
            {
                result.Add(identity, index);
            }
        }

        return result;
    }

    private static Dictionary<string, IReadOnlyList<int>> CreateContentHashMap(
        IReadOnlyList<BlockIdentity> blocks) =>
        blocks
            .Select((block, index) => (block.ContentHash, Index: index))
            .GroupBy(item => item.ContentHash, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<int>)group.Select(item => item.Index).ToArray(),
                StringComparer.Ordinal);

    private static void MatchStrongIdentities(
        IReadOnlyList<BlockIdentity> baseline,
        IReadOnlyList<BlockIdentity> candidate,
        IReadOnlyDictionary<string, int> baselineByExplicitId,
        IReadOnlyDictionary<string, int> baselineByGeneratedId,
        IReadOnlyDictionary<string, IReadOnlyList<int>> baselineByContentHash,
        MatchResult[] matches,
        bool[] matchedBaseline)
    {
        for (var candidateIndex = 0; candidateIndex < candidate.Count; candidateIndex++)
        {
            var block = candidate[candidateIndex];
            var explicitIndex = ResolveIdentity(block.ExplicitId, baselineByExplicitId);
            var generatedIndex = ResolveIdentity(block.GeneratedId, baselineByGeneratedId);

            if (explicitIndex is not null &&
                generatedIndex is not null &&
                explicitIndex != generatedIndex)
            {
                throw IdentityConflict(candidateIndex);
            }

            var baselineIndex = explicitIndex ?? generatedIndex;
            if (baselineIndex is null)
            {
                continue;
            }

            if (baselineByContentHash.TryGetValue(block.ContentHash, out var hashCandidates) &&
                !hashCandidates.Contains(baselineIndex.Value))
            {
                throw IdentityConflict(candidateIndex);
            }

            if (matchedBaseline[baselineIndex.Value])
            {
                throw IdentityConflict(candidateIndex);
            }

            matches[candidateIndex] = new MatchResult(
                baselineIndex.Value,
                explicitIndex is not null ? BlockMatchKind.ExplicitId : BlockMatchKind.GeneratedId);
            matchedBaseline[baselineIndex.Value] = true;
        }
    }

    private static int? ResolveIdentity(
        string? identity,
        IReadOnlyDictionary<string, int> baselineByIdentity) =>
        identity is not null && baselineByIdentity.TryGetValue(identity, out var index)
            ? index
            : null;

    private static DiffConflictException IdentityConflict(int candidateIndex) => new(
        DiffErrorCodes.IdentityConflict,
        $"Candidate block {candidateIndex} resolves to conflicting baseline blocks.");

    private static void MatchUnambiguousContentHashes(
        IReadOnlyList<BlockIdentity> baseline,
        IReadOnlyList<BlockIdentity> candidate,
        MatchResult[] matches,
        bool[] matchedBaseline)
    {
        var processedHashes = new HashSet<string>(StringComparer.Ordinal);
        for (var candidateIndex = 0; candidateIndex < candidate.Count; candidateIndex++)
        {
            if (matches[candidateIndex].IsMatched)
            {
                continue;
            }

            var contentHash = candidate[candidateIndex].ContentHash;
            if (!processedHashes.Add(contentHash))
            {
                continue;
            }

            var candidateIndexes = Enumerable.Range(0, candidate.Count)
                .Where(index =>
                    !matches[index].IsMatched &&
                    string.Equals(candidate[index].ContentHash, contentHash, StringComparison.Ordinal))
                .ToArray();
            var baselineIndexes = Enumerable.Range(0, baseline.Count)
                .Where(index =>
                    !matchedBaseline[index] &&
                    string.Equals(baseline[index].ContentHash, contentHash, StringComparison.Ordinal))
                .ToArray();

            if (baselineIndexes.Length == 0)
            {
                continue;
            }

            if (candidateIndexes.Length != 1 || baselineIndexes.Length != 1)
            {
                throw new DiffConflictException(
                    DiffErrorCodes.ContentHashAmbiguous,
                    $"ContentHash fallback is ambiguous for hash: {contentHash}");
            }

            matches[candidateIndexes[0]] = new MatchResult(
                baselineIndexes[0],
                BlockMatchKind.ContentHash);
            matchedBaseline[baselineIndexes[0]] = true;
        }
    }

    private static IReadOnlyList<DiffOperation> BuildOperations(
        IReadOnlyList<BlockIdentity> baseline,
        IReadOnlyList<BlockIdentity> candidate,
        MatchResult[] matches)
    {
        var operations = new List<DiffOperation>();
        var matchedBaseline = new bool[baseline.Count];
        var movedCandidate = FindMovedCandidateBlocks(matches);

        for (var candidateIndex = 0; candidateIndex < candidate.Count; candidateIndex++)
        {
            var match = matches[candidateIndex];
            if (!match.IsMatched)
            {
                operations.Add(new DiffOperation(
                    DiffOperationKind.Insert,
                    null,
                    candidateIndex,
                    null,
                    candidate[candidateIndex],
                    null));
                continue;
            }

            var baselineIndex = match.PreviousIndex;
            matchedBaseline[baselineIndex] = true;
            var contentChanged = !string.Equals(
                baseline[baselineIndex].ContentHash,
                candidate[candidateIndex].ContentHash,
                StringComparison.Ordinal);

            if (movedCandidate[candidateIndex])
            {
                operations.Add(new DiffOperation(
                    DiffOperationKind.Move,
                    baselineIndex,
                    candidateIndex,
                    baseline[baselineIndex],
                    candidate[candidateIndex],
                    match.Kind));
            }

            if (contentChanged)
            {
                operations.Add(new DiffOperation(
                    DiffOperationKind.Update,
                    baselineIndex,
                    candidateIndex,
                    baseline[baselineIndex],
                    candidate[candidateIndex],
                    match.Kind));
            }

            if (!movedCandidate[candidateIndex] && !contentChanged)
            {
                operations.Add(new DiffOperation(
                    DiffOperationKind.NoChange,
                    baselineIndex,
                    candidateIndex,
                    baseline[baselineIndex],
                    candidate[candidateIndex],
                    match.Kind));
            }
        }

        for (var baselineIndex = 0; baselineIndex < baseline.Count; baselineIndex++)
        {
            if (!matchedBaseline[baselineIndex])
            {
                operations.Add(new DiffOperation(
                    DiffOperationKind.Delete,
                    baselineIndex,
                    null,
                    baseline[baselineIndex],
                    null,
                    null));
            }
        }

        return operations;
    }

    private static bool[] FindMovedCandidateBlocks(MatchResult[] matches)
    {
        var matchedCandidateIndexes = Enumerable.Range(0, matches.Length)
            .Where(index => matches[index].IsMatched)
            .ToArray();
        var movedCandidate = new bool[matches.Length];
        if (matchedCandidateIndexes.Length == 0)
        {
            return movedCandidate;
        }

        var tailSequenceIndexes = new int[matchedCandidateIndexes.Length];
        var predecessorSequenceIndexes = Enumerable.Repeat(-1, matchedCandidateIndexes.Length).ToArray();
        var longestLength = 0;

        for (var sequenceIndex = 0; sequenceIndex < matchedCandidateIndexes.Length; sequenceIndex++)
        {
            var baselineIndex = matches[matchedCandidateIndexes[sequenceIndex]].PreviousIndex;
            var lower = 0;
            var upper = longestLength;
            while (lower < upper)
            {
                var middle = lower + ((upper - lower) / 2);
                var tailBaselineIndex = matches[
                    matchedCandidateIndexes[tailSequenceIndexes[middle]]].PreviousIndex;
                if (tailBaselineIndex < baselineIndex)
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

        var stableSequenceIndexes = new bool[matchedCandidateIndexes.Length];
        for (var sequenceIndex = tailSequenceIndexes[longestLength - 1];
             sequenceIndex >= 0;
             sequenceIndex = predecessorSequenceIndexes[sequenceIndex])
        {
            stableSequenceIndexes[sequenceIndex] = true;
        }

        for (var sequenceIndex = 0; sequenceIndex < matchedCandidateIndexes.Length; sequenceIndex++)
        {
            if (!stableSequenceIndexes[sequenceIndex])
            {
                movedCandidate[matchedCandidateIndexes[sequenceIndex]] = true;
            }
        }

        return movedCandidate;
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
