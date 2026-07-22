using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;
using System.Security.Cryptography;
using System.Text;

namespace Vmf.Publisher.UnitTests;

public sealed class DiffEngineTests
{
    private readonly DiffEngine engine = new();

    [Fact]
    public void CreatePlan_WithoutBaseline_InsertsEveryBlock()
    {
        var candidate = Candidate(
            "current",
            Block("one", "generated-one", "hash-one"),
            Block("two", "generated-two", "hash-two"));

        var plan = engine.CreatePlan(null, candidate);

        Assert.True(plan.IsPublishRequired);
        Assert.False(plan.IsFingerprintMatch);
        Assert.Collection(
            plan.Operations,
            operation => AssertOperation(operation, DiffOperationKind.Insert, null, 0, null),
            operation => AssertOperation(operation, DiffOperationKind.Insert, null, 1, null));
    }

    [Fact]
    public void CreatePlan_WithSameFingerprint_SkipsMutationAfterIdentityValidation()
    {
        var baseline = Baseline("same", Block("one", null, "old-hash"));
        var candidate = Candidate("same", Block("one", null, "new-hash"));

        var plan = engine.CreatePlan(baseline, candidate);

        Assert.True(plan.IsFingerprintMatch);
        Assert.False(plan.IsPublishRequired);
        Assert.Empty(plan.Operations);
    }

    [Fact]
    public void CreatePlan_SameFingerprintDoesNotBypassCrossTierConflict()
    {
        var baseline = Baseline(
            "same",
            Block("explicit", null, "hash-a"),
            Block(null, "generated", "hash-b"));
        var candidate = Candidate("same", Block("explicit", "generated", "new-hash"));

        AssertConflict(
            DiffErrorCodes.IdentityConflict,
            () => engine.CreatePlan(baseline, candidate));
    }

    [Fact]
    public void CreatePlan_SameFingerprintDoesNotBypassContentHashAmbiguity()
    {
        var baseline = Baseline(
            "same",
            Block(null, null, "duplicate"),
            Block(null, null, "duplicate"));
        var candidate = Candidate("same", Block(null, null, "duplicate"));

        AssertConflict(
            DiffErrorCodes.ContentHashAmbiguous,
            () => engine.CreatePlan(baseline, candidate));
    }

    [Fact]
    public void ContentHash_OneBaselineAndOneCandidate_MatchesUniquely()
    {
        var plan = engine.CreatePlan(
            Baseline("previous", Block(null, null, "same")),
            Candidate("current", Block(null, null, "same")));

        var operation = Assert.Single(plan.Operations);
        AssertOperation(operation, DiffOperationKind.NoChange, 0, 0, BlockMatchKind.ContentHash);
    }

    [Fact]
    public void ContentHash_MultipleBaselineCandidates_StopsSafely()
    {
        var baseline = Baseline(
            "previous",
            Block(null, null, "duplicate"),
            Block(null, null, "duplicate"));
        var candidate = Candidate("current", Block(null, null, "duplicate"));

        AssertConflict(
            DiffErrorCodes.ContentHashAmbiguous,
            () => engine.CreatePlan(baseline, candidate));
    }

    [Fact]
    public void ContentHash_MultipleCandidateBlocks_StopsSafely()
    {
        var baseline = Baseline("previous", Block(null, null, "duplicate"));
        var candidate = Candidate(
            "current",
            Block(null, null, "duplicate"),
            Block(null, null, "duplicate"));

        AssertConflict(
            DiffErrorCodes.ContentHashAmbiguous,
            () => engine.CreatePlan(baseline, candidate));
    }

    [Fact]
    public void ContentHash_MultipleBlocksOnBothSides_StopsSafely()
    {
        var baseline = Baseline(
            "previous",
            Block(null, null, "duplicate"),
            Block(null, null, "duplicate"));
        var candidate = Candidate(
            "current",
            Block(null, null, "duplicate"),
            Block(null, null, "duplicate"));

        AssertConflict(
            DiffErrorCodes.ContentHashAmbiguous,
            () => engine.CreatePlan(baseline, candidate));
    }

    [Fact]
    public void ContentHash_StrongIdentityReservationLeavesOneUniqueFallback()
    {
        var baseline = Baseline(
            "previous",
            Block("explicit", null, "duplicate"),
            Block(null, null, "duplicate"));
        var candidate = Candidate(
            "current",
            Block("explicit", null, "duplicate"),
            Block(null, null, "duplicate"));

        var plan = engine.CreatePlan(baseline, candidate);

        Assert.Collection(
            plan.Operations,
            operation => AssertOperation(
                operation,
                DiffOperationKind.NoChange,
                0,
                0,
                BlockMatchKind.ExplicitId),
            operation => AssertOperation(
                operation,
                DiffOperationKind.NoChange,
                1,
                1,
                BlockMatchKind.ContentHash));
    }

    [Fact]
    public void ContentHash_UniqueFallbackCanParticipateInMoveWithDuplicateHash()
    {
        var baseline = Baseline(
            "previous",
            Block("first", null, "duplicate"),
            Block(null, null, "duplicate"),
            Block("third", null, "third-hash"));
        var candidate = Candidate(
            "current",
            Block("third", null, "third-hash"),
            Block(null, null, "duplicate"),
            Block("first", null, "duplicate"));

        var plan = engine.CreatePlan(baseline, candidate);

        Assert.Contains(plan.Operations, operation =>
            operation.Kind == DiffOperationKind.Move &&
            operation.MatchKind == BlockMatchKind.ContentHash &&
            operation.PreviousIndex == 1 &&
            operation.CurrentIndex == 1);
    }

    [Fact]
    public void Identity_ExplicitAndGeneratedResolveSameBaselineBlock()
    {
        var baseline = Baseline("previous", Block("explicit", "generated", "hash"));
        var candidate = Candidate("current", Block("explicit", "generated", "hash"));

        var operation = Assert.Single(engine.CreatePlan(baseline, candidate).Operations);

        AssertOperation(operation, DiffOperationKind.NoChange, 0, 0, BlockMatchKind.ExplicitId);
    }

    [Fact]
    public void Identity_GeneratedIdPrecedesContentHashWhenConflictFree()
    {
        var baseline = Baseline("previous", Block(null, "generated", "hash"));
        var candidate = Candidate("current", Block(null, "generated", "hash"));

        var operation = Assert.Single(engine.CreatePlan(baseline, candidate).Operations);

        AssertOperation(operation, DiffOperationKind.NoChange, 0, 0, BlockMatchKind.GeneratedId);
    }

    [Fact]
    public void Identity_ExplicitAndGeneratedResolveDifferentBlocks_StopsSafely()
    {
        var baseline = Baseline(
            "previous",
            Block("explicit", null, "hash-a"),
            Block(null, "generated", "hash-b"));
        var candidate = Candidate("current", Block("explicit", "generated", "new-hash"));

        AssertConflict(
            DiffErrorCodes.IdentityConflict,
            () => engine.CreatePlan(baseline, candidate));
    }

    [Fact]
    public void Identity_ExplicitAndContentHashResolveDifferentBlocks_StopsSafely()
    {
        var baseline = Baseline(
            "previous",
            Block("explicit", null, "hash-a"),
            Block(null, null, "hash-b"));
        var candidate = Candidate("current", Block("explicit", null, "hash-b"));

        AssertConflict(
            DiffErrorCodes.IdentityConflict,
            () => engine.CreatePlan(baseline, candidate));
    }

    [Fact]
    public void Identity_GeneratedAndContentHashResolveDifferentBlocks_StopsSafely()
    {
        var baseline = Baseline(
            "previous",
            Block(null, "generated", "hash-a"),
            Block(null, null, "hash-b"));
        var candidate = Candidate("current", Block(null, "generated", "hash-b"));

        AssertConflict(
            DiffErrorCodes.IdentityConflict,
            () => engine.CreatePlan(baseline, candidate));
    }

    [Fact]
    public void Identity_AllThreeTiersResolveDifferentBlocks_StopsSafely()
    {
        var baseline = Baseline(
            "previous",
            Block("explicit", null, "hash-a"),
            Block(null, "generated", "hash-b"),
            Block(null, null, "hash-c"));
        var candidate = Candidate("current", Block("explicit", "generated", "hash-c"));

        AssertConflict(
            DiffErrorCodes.IdentityConflict,
            () => engine.CreatePlan(baseline, candidate));
    }

    [Fact]
    public void Identity_TwoCandidateBlocksCannotConsumeOneBaselineAcrossTiers()
    {
        var baseline = Baseline("previous", Block("explicit", "generated", "hash"));
        var candidate = Candidate(
            "current",
            Block("explicit", null, "changed-one"),
            Block(null, "generated", "changed-two"));

        AssertConflict(
            DiffErrorCodes.IdentityConflict,
            () => engine.CreatePlan(baseline, candidate));
    }

    [Fact]
    public void Identity_DuplicateExplicitId_UsesStableConflictCode()
    {
        var candidate = Candidate(
            "current",
            Block("duplicate", null, "hash-one"),
            Block("duplicate", null, "hash-two"));

        AssertConflict(
            DiffErrorCodes.DuplicateIdentity,
            () => engine.CreatePlan(null, candidate));
    }

    [Fact]
    public void Identity_DuplicateGeneratedId_UsesStableConflictCode()
    {
        var baseline = Baseline(
            "previous",
            Block(null, "duplicate", "hash-one"),
            Block(null, "duplicate", "hash-two"));

        AssertConflict(
            DiffErrorCodes.DuplicateIdentity,
            () => engine.CreatePlan(baseline, Candidate("current")));
    }

    [Fact]
    public void DocumentIdentity_PublicationIdMismatch_StopsSafely()
    {
        AssertDocumentIdentityMismatch(
            Identity("publication-a", "document", "google"),
            Identity("publication-b", "document", "google"));
    }

    [Fact]
    public void DocumentIdentity_DocumentIdMismatch_StopsSafely()
    {
        AssertDocumentIdentityMismatch(
            Identity("publication", "document-a", "google"),
            Identity("publication", "document-b", "google"));
    }

    [Fact]
    public void DocumentIdentity_GoogleDocumentIdMismatch_StopsSafely()
    {
        AssertDocumentIdentityMismatch(
            Identity("publication", "document", "google-a"),
            Identity("publication", "document", "google-b"));
    }

    [Fact]
    public void DocumentIdentity_BothGoogleDocumentIdsNull_AreAllowed()
    {
        var plan = engine.CreatePlan(
            Baseline("previous", Identity("publication", "document", null)),
            Candidate("current", Identity("publication", "document", null)));

        Assert.Empty(plan.Operations);
    }

    [Fact]
    public void DocumentIdentity_OnlyBaselineGoogleDocumentIdNull_StopsSafely()
    {
        AssertDocumentIdentityMismatch(
            Identity("publication", "document", null),
            Identity("publication", "document", "google"));
    }

    [Fact]
    public void DocumentIdentity_OnlyCandidateGoogleDocumentIdNull_StopsSafely()
    {
        AssertDocumentIdentityMismatch(
            Identity("publication", "document", "google"),
            Identity("publication", "document", null));
    }

    [Fact]
    public void DocumentIdentity_SameGoogleDocumentId_IsAllowed()
    {
        var plan = engine.CreatePlan(
            Baseline("previous", Identity("publication", "document", "google")),
            Candidate("current", Identity("publication", "document", "google")));

        Assert.Empty(plan.Operations);
    }

    [Fact]
    public void DocumentIdentity_UsesOrdinalComparison()
    {
        AssertDocumentIdentityMismatch(
            Identity("Publication", "document", "google"),
            Identity("publication", "document", "google"));
    }

    [Fact]
    public void Move_ThreeBlockCycle_UsesOneLogicalMove()
    {
        var baselineBlocks = Blocks("a", "b", "c");
        BlockIdentity[] candidateBlocks = [baselineBlocks[1], baselineBlocks[2], baselineBlocks[0]];

        var plan = engine.CreatePlan(
            Baseline("previous", baselineBlocks),
            Candidate("current", candidateBlocks));

        var move = Assert.Single(plan.Operations.Where(operation => operation.Kind == DiffOperationKind.Move));
        AssertOperation(move, DiffOperationKind.Move, 0, 2, BlockMatchKind.ExplicitId);
    }

    [Fact]
    public void Move_CompleteReverse_MovesAllButOneLISBlock()
    {
        var baselineBlocks = Blocks("a", "b", "c", "d");
        var candidateBlocks = baselineBlocks.Reverse().ToArray();

        var plan = engine.CreatePlan(
            Baseline("previous", baselineBlocks),
            Candidate("current", candidateBlocks));

        Assert.Equal(3, plan.Operations.Count(operation => operation.Kind == DiffOperationKind.Move));
    }

    [Fact]
    public void Move_MultipleLongestSubsequences_UsesDeterministicTailReplacement()
    {
        var baselineBlocks = Blocks("a", "b", "c", "d");
        var candidateBlocks =
            new[] { baselineBlocks[1], baselineBlocks[0], baselineBlocks[3], baselineBlocks[2] };

        var plan = engine.CreatePlan(
            Baseline("previous", baselineBlocks),
            Candidate("current", candidateBlocks));

        Assert.Equal(
            [0, 2],
            plan.Operations
                .Where(operation => operation.Kind == DiffOperationKind.Move)
                .Select(operation => operation.CurrentIndex));
    }

    [Fact]
    public void Move_HeadDeletion_DoesNotMoveFollowingBlocks()
    {
        AssertDeletionDoesNotMove(Blocks("a", "b", "c"), ["b", "c"]);
    }

    [Fact]
    public void Move_MiddleDeletion_DoesNotMoveFollowingBlocks()
    {
        AssertDeletionDoesNotMove(Blocks("a", "b", "c"), ["a", "c"]);
    }

    [Fact]
    public void Move_MultipleDeletions_DoNotMoveRemainingBlocks()
    {
        AssertDeletionDoesNotMove(Blocks("a", "b", "c", "d", "e"), ["b", "e"]);
    }

    [Fact]
    public void Move_InsertAndDeleteTogether_DoNotCreateFalseMove()
    {
        var baseline = Blocks("a", "b", "c");
        var candidate = new[] { Block("x", null, "hash-x"), baseline[0], baseline[2] };

        var plan = engine.CreatePlan(Baseline("previous", baseline), Candidate("current", candidate));

        Assert.DoesNotContain(plan.Operations, operation => operation.Kind == DiffOperationKind.Move);
        Assert.Single(plan.Operations, operation => operation.Kind == DiffOperationKind.Insert);
        Assert.Single(plan.Operations, operation => operation.Kind == DiffOperationKind.Delete);
    }

    [Fact]
    public void Move_RelocatedChangedBlock_ProducesMoveThenUpdate()
    {
        var baseline = Baseline(
            "previous",
            Block("one", null, "hash-one"),
            Block("two", null, "old-hash-two"));
        var candidate = Candidate(
            "current",
            Block("two", null, "new-hash-two"),
            Block("one", null, "hash-one"));

        var plan = engine.CreatePlan(baseline, candidate);

        Assert.Equal(DiffOperationKind.Move, plan.Operations[0].Kind);
        Assert.Equal(DiffOperationKind.Update, plan.Operations[1].Kind);
        Assert.Equal(1, plan.Operations[0].PreviousIndex);
        Assert.Equal(0, plan.Operations[0].CurrentIndex);
    }

    [Fact]
    public void Move_IndexShiftCausedByInsertion_DoesNotMoveFollowingBlocks()
    {
        var baseline = Blocks("one", "two");
        var candidate = new[] { Block("inserted", null, "hash-inserted"), baseline[0], baseline[1] };

        var plan = engine.CreatePlan(Baseline("previous", baseline), Candidate("current", candidate));

        Assert.DoesNotContain(plan.Operations, operation => operation.Kind == DiffOperationKind.Move);
    }

    [Fact]
    public void Idempotency_DifferentFingerprintsWithUnchangedBlocks_DoNotRequirePublish()
    {
        var blocks = Blocks("a", "b");

        var plan = engine.CreatePlan(
            Baseline("previous", blocks),
            Candidate("current", blocks));

        Assert.False(plan.IsFingerprintMatch);
        Assert.False(plan.IsPublishRequired);
        Assert.All(plan.Operations, operation => Assert.Equal(DiffOperationKind.NoChange, operation.Kind));
    }

    [Fact]
    public void Determinism_SameInputProducesSameOperationSequence()
    {
        var baseline = Baseline("previous", Blocks("a", "b", "c"));
        var candidate = Candidate("current", Blocks("c", "a", "d"));

        var first = engine.CreatePlan(baseline, candidate);
        var second = engine.CreatePlan(baseline, candidate);

        Assert.Equal(Signatures(first), Signatures(second));
    }

    [Fact]
    public void Idempotency_EmptyDocumentsProduceEmptyPlan()
    {
        var plan = engine.CreatePlan(Baseline("previous"), Candidate("current"));

        Assert.Empty(plan.Operations);
        Assert.False(plan.IsPublishRequired);
    }

    [Fact]
    public void Idempotency_EmptyBaselineProducesAllInserts()
    {
        var plan = engine.CreatePlan(
            Baseline("previous"),
            Candidate("current", Blocks("a", "b")));

        Assert.All(plan.Operations, operation => Assert.Equal(DiffOperationKind.Insert, operation.Kind));
        Assert.Equal(2, plan.Operations.Count);
    }

    [Fact]
    public void Idempotency_EmptyCandidateProducesAllDeletes()
    {
        var plan = engine.CreatePlan(
            Baseline("previous", Blocks("a", "b")),
            Candidate("current"));

        Assert.All(plan.Operations, operation => Assert.Equal(DiffOperationKind.Delete, operation.Kind));
        Assert.Equal(2, plan.Operations.Count);
    }

    [Fact]
    public void DeterministicPermutations_PreserveOneToOneMappingAndCandidateOrder()
    {
        var baselineBlocks = Blocks("a", "b", "c", "d");
        foreach (var permutation in Permutations(baselineBlocks))
        {
            var baseline = Baseline("previous", baselineBlocks);
            var candidate = Candidate("current", permutation);

            var first = engine.CreatePlan(baseline, candidate);
            var second = engine.CreatePlan(baseline, candidate);

            Assert.Equal(Signatures(first), Signatures(second));
            Assert.Equal(4, first.Operations.Count);
            Assert.Equal(4, first.Operations.Select(operation => operation.PreviousIndex).Distinct().Count());
            Assert.Equal(4, first.Operations.Select(operation => operation.CurrentIndex).Distinct().Count());
            Assert.Equal(
                permutation.Select(block => block.ExplicitId),
                ApplyLogicalPlanOrder(first, permutation.Length));
        }
    }

    [Fact]
    public void DeterministicMixedPlan_CoversEveryBaselineAndCandidateExactlyOnce()
    {
        var baselineBlocks = Blocks("a", "b", "c", "d");
        var candidateBlocks = new[]
        {
            Block("x", null, "hash-x"),
            baselineBlocks[2],
            baselineBlocks[0],
            Block("y", null, "hash-y"),
        };

        var plan = engine.CreatePlan(
            Baseline("previous", baselineBlocks),
            Candidate("current", candidateBlocks));
        var currentGroups = plan.Operations
            .Where(operation => operation.CurrentIndex is not null)
            .GroupBy(operation => operation.CurrentIndex!.Value)
            .ToArray();
        var consumedBaseline = plan.Operations
            .Where(operation => operation.PreviousIndex is not null)
            .Select(operation => operation.PreviousIndex!.Value)
            .Distinct()
            .ToArray();

        Assert.Equal(candidateBlocks.Length, currentGroups.Length);
        Assert.Equal(baselineBlocks.Length, consumedBaseline.Length);
        Assert.Equal(
            candidateBlocks.Select(block => block.ExplicitId),
            ApplyLogicalPlanOrder(plan, candidateBlocks.Length));
        Assert.Contains(plan.Operations, operation => operation.Kind == DiffOperationKind.Insert);
        Assert.Contains(plan.Operations, operation => operation.Kind == DiffOperationKind.Delete);
        Assert.Contains(plan.Operations, operation => operation.Kind == DiffOperationKind.Move);
    }

    private void AssertDeletionDoesNotMove(
        BlockIdentity[] baselineBlocks,
        IReadOnlyList<string> remainingIds)
    {
        var byId = baselineBlocks.ToDictionary(block => block.ExplicitId!, StringComparer.Ordinal);
        var candidateBlocks = remainingIds.Select(id => byId[id]).ToArray();

        var plan = engine.CreatePlan(
            Baseline("previous", baselineBlocks),
            Candidate("current", candidateBlocks));

        Assert.DoesNotContain(plan.Operations, operation => operation.Kind == DiffOperationKind.Move);
    }

    private void AssertDocumentIdentityMismatch(DocumentIdentity baseline, DocumentIdentity candidate)
    {
        AssertConflict(
            DiffErrorCodes.DocumentIdentityMismatch,
            () => engine.CreatePlan(Baseline("previous", baseline), Candidate("current", candidate)));
    }

    private static VerifiedPublishState Baseline(
        string fingerprint,
        params BlockIdentity[] blocks) =>
        Baseline(fingerprint, Identity(), blocks);

    private static VerifiedPublishState Baseline(
        string fingerprint,
        DocumentIdentity identity,
        params BlockIdentity[] blocks) =>
        new(identity, Fingerprint(fingerprint), blocks);

    private static PublishCandidate Candidate(
        string fingerprint,
        params BlockIdentity[] blocks) =>
        Candidate(fingerprint, Identity(), blocks);

    private static PublishCandidate Candidate(
        string fingerprint,
        DocumentIdentity identity,
        params BlockIdentity[] blocks) =>
        new(identity, Fingerprint(fingerprint), blocks);

    private static DocumentIdentity Identity(
        string publicationId = "publication",
        string documentId = "document",
        string? googleDocumentId = "google-document") =>
        new(publicationId, documentId, googleDocumentId, DocumentState.Existing);

    private static BlockIdentity Block(string? explicitId, string? generatedId, string hash) =>
        new(explicitId, generatedId, hash);

    private static BlockIdentity[] Blocks(params string[] ids) =>
        ids.Select(id => Block(id, null, $"hash-{id}")).ToArray();

    private static PublishFingerprint Fingerprint(string seed) =>
        new("v1:sha256:" + Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(seed))).ToLowerInvariant());

    private static void AssertConflict(string expectedCode, Action action)
    {
        var exception = Assert.Throws<DiffConflictException>(action);
        Assert.Equal(expectedCode, exception.Code);
    }

    private static void AssertOperation(
        DiffOperation operation,
        DiffOperationKind kind,
        int? previousIndex,
        int? currentIndex,
        BlockMatchKind? matchKind)
    {
        Assert.Equal(kind, operation.Kind);
        Assert.Equal(previousIndex, operation.PreviousIndex);
        Assert.Equal(currentIndex, operation.CurrentIndex);
        Assert.Equal(matchKind, operation.MatchKind);
    }

    private static string[] Signatures(DiffPlan plan) =>
        plan.Operations.Select(operation => string.Join(
            ":",
            operation.Kind,
            operation.PreviousIndex?.ToString() ?? "-",
            operation.CurrentIndex?.ToString() ?? "-",
            operation.MatchKind?.ToString() ?? "-")).ToArray();

    private static IReadOnlyList<string?> ApplyLogicalPlanOrder(DiffPlan plan, int candidateCount)
    {
        var result = new string?[candidateCount];
        foreach (var group in plan.Operations
                     .Where(operation => operation.CurrentIndex is not null)
                     .GroupBy(operation => operation.CurrentIndex!.Value))
        {
            result[group.Key] = group
                .Select(operation => operation.CurrentBlock?.ExplicitId)
                .First(id => id is not null);
        }

        Assert.DoesNotContain(result, id => id is null);
        return result;
    }

    private static IEnumerable<BlockIdentity[]> Permutations(IReadOnlyList<BlockIdentity> source)
    {
        if (source.Count == 0)
        {
            yield return [];
            yield break;
        }

        for (var index = 0; index < source.Count; index++)
        {
            var remaining = source.Where((_, itemIndex) => itemIndex != index).ToArray();
            foreach (var tail in Permutations(remaining))
            {
                yield return [source[index], .. tail];
            }
        }
    }
}
