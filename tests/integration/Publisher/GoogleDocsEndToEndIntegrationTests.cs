using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Text.Json;
using Vmf.Publisher.Application;
using Vmf.Publisher.Composition;
using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure;
using Vmf.Publisher.Infrastructure.Google;

namespace Vmf.Publisher.IntegrationTests;

[Collection("GoogleDocsEndToEndLive")]
public sealed class GoogleDocsEndToEndIntegrationTests
{
    private const string EnableVariable = "VMF_PUBLISHER_GOOGLE_E2E";
    private const string CredentialsPathVariable = "VMF_PUBLISHER_GOOGLE_CREDENTIALS_PATH";
    private const string TokenStorePathVariable = "VMF_PUBLISHER_GOOGLE_TOKEN_STORE_PATH";
    private const string AuthModeVariable = "VMF_PUBLISHER_GOOGLE_AUTH_MODE";
    private const string TemplateDocumentIdVariable = "VMF_PUBLISHER_GOOGLE_E2E_TEMPLATE_DOCUMENT_ID";
    private const string FolderIdVariable = "VMF_PUBLISHER_GOOGLE_E2E_FOLDER_ID";

    [Fact]
    public async Task Success_AppliesReadsBackVerifiesAndAllowsVerifiedStateCommit()
    {
        var context = await GoogleEndToEndContext.TryCreateAsync(default);
        if (context is null)
        {
            return;
        }

        await using (context)
        {
            var baseline = await context.ResetAsync("intro", "Before.", default);
            var candidate = context.CreateCandidate(("intro", "After."));
            var plan = await context.CreatePhysicalPlanAsync(baseline, candidate, default);

            var result = await context.ApplyAndVerifyAsync(baseline, candidate, plan, default);

            Assert.True(result.ApplyResult.Applied, Describe(result.ApplyResult));
            Assert.True(result.VerificationSuccess, Describe(result));
            Assert.True(result.VerifiedStateCommitAllowed, Describe(result));
            Assert.Equal(plan.RequiredRevision.RevisionId, result.ApplyResult.RequiredRevisionId);
            Assert.NotNull(result.ApplyResult.AppliedRevisionId);
            Assert.NotEqual(plan.RequiredRevision.RevisionId, result.ApplyResult.AppliedRevisionId);
            Assert.Equal(result.ApplyResult.AppliedRevisionId, result.Readback.Revision.RevisionId);
            Assert.True(result.Readback.Revision.Sequence > baseline.Revision.Sequence);
        }
    }

    [Fact]
    public async Task RevisionConflict_ReturnsConflictBeforeVerification()
    {
        var context = await GoogleEndToEndContext.TryCreateAsync(default);
        if (context is null)
        {
            return;
        }

        await using (context)
        {
            var baseline = await context.ResetAsync("intro", "Before.", default);
            var candidate = context.CreateCandidate(("intro", "After."));
            var plan = await context.CreatePhysicalPlanAsync(baseline, candidate, default);
            await context.AppendOutsideManagedRegionAsync("external conflict", default);

            var apply = await context.Executor.ExecuteAsync(plan, dryRun: false, default);

            Assert.True(apply.Conflict, Describe(apply));
            Assert.Equal(UpdateErrorCodes.RevisionConflict, apply.ErrorCode);
            Assert.False(context.VerifyAfterApply(baseline, candidate, plan.LogicalPlan, apply).VerifiedStateCommitAllowed);
        }
    }

    [Fact]
    public async Task ReadbackMismatch_DisallowsVerifiedStateCommit()
    {
        var context = await GoogleEndToEndContext.TryCreateAsync(default);
        if (context is null)
        {
            return;
        }

        await using (context)
        {
            var baseline = await context.ResetAsync("intro", "Before.", default);
            var candidate = context.CreateCandidate(("intro", "After."));
            var plan = await context.CreatePhysicalPlanAsync(baseline, candidate, default);
            var apply = await context.Executor.ExecuteAsync(plan, dryRun: false, default);
            Assert.True(apply.Applied, Describe(apply));

            await context.ReplaceManagedTextAsync("Tampered.\n", default);
            var result = context.VerifyAfterApply(baseline, candidate, plan.LogicalPlan, apply);

            Assert.False(result.VerificationSuccess);
            Assert.False(result.VerifiedStateCommitAllowed);
            Assert.NotNull(result.VerificationErrorCode);
        }
    }

    [Fact]
    public async Task EmptyPlan_DoesNotCallGoogleDocsBatchUpdateAndStillVerifies()
    {
        var context = await GoogleEndToEndContext.TryCreateAsync(default);
        if (context is null)
        {
            return;
        }

        await using (context)
        {
            var baseline = await context.ResetAsync("intro", "Stable.", default);
            var candidate = context.CreateCandidate(("intro", "Stable."));
            var plan = await context.CreatePhysicalPlanAsync(baseline, candidate, default);

            var result = await context.ApplyAndVerifyAsync(baseline, candidate, plan, default);

            Assert.Equal(PhysicalUpdateExecutionStatus.NoChange, result.ApplyResult.Status);
            Assert.Equal(0, context.BatchUpdateCallCount);
            Assert.True(result.VerificationSuccess);
            Assert.True(result.VerifiedStateCommitAllowed);
            Assert.Equal(baseline.Revision, result.Readback.Revision);
        }
    }

    private sealed class GoogleEndToEndContext : IAsyncDisposable, IGoogleDocsGateway
    {
        private readonly HttpClient httpClient;
        private readonly IGoogleCredentialProvider credentialProvider;
        private readonly GoogleDocsDocumentReader reader;
        private readonly string documentId;
        private readonly string stateRoot;
        private readonly Dictionary<string, long> revisionSequences = new(StringComparer.Ordinal);
        private PublishCandidate? markerlessReadbackCandidate;
        private long sequence;

        private GoogleEndToEndContext(
            HttpClient httpClient,
            IGoogleCredentialProvider credentialProvider,
            string documentId)
        {
            this.httpClient = httpClient;
            this.credentialProvider = credentialProvider;
            this.documentId = documentId;
            stateRoot = Path.Combine(
                Path.GetTempPath(),
                "vmf-publisher-google-e2e-state-" + Guid.NewGuid().ToString("N"));
            reader = new GoogleDocsDocumentReader(this, documentId);
            Executor = new GoogleDocsPhysicalPlanExecutor(new GoogleDocsBatchRequestBuilder(), this);
        }

        internal GoogleDocsPhysicalPlanExecutor Executor { get; }

        internal int BatchUpdateCallCount { get; private set; }

        public async ValueTask DisposeAsync()
        {
            await DeleteDocumentAsync(default).ConfigureAwait(false);
            if (Directory.Exists(stateRoot))
            {
                Directory.Delete(stateRoot, recursive: true);
            }

            httpClient.Dispose();
        }

        public async Task<BatchUpdateDocumentResponse> BatchUpdateDocumentAsync(
            string targetDocumentId,
            BatchUpdateDocumentRequest request,
            CancellationToken cancellationToken)
        {
            BatchUpdateCallCount++;
            string response;
            try
            {
                response = await SendAsync(
                    HttpMethod.Post,
                    $"https://docs.googleapis.com/v1/documents/{Uri.EscapeDataString(targetDocumentId)}:batchUpdate",
                    JsonSerializer.Serialize(new
                    {
                        requests = request.Requests,
                        writeControl = new
                        {
                            requiredRevisionId = request.WriteControl.RequiredRevisionId,
                        },
                    }),
                    cancellationToken).ConfigureAwait(false);
            }
            catch (GoogleDocsBatchUpdateException exception)
                when (exception.HttpStatusCode == HttpStatusCode.BadRequest)
            {
                var latest = await GetDocumentBodyAsync(cancellationToken).ConfigureAwait(false);
                if (!string.Equals(
                        latest.RevisionId,
                        request.WriteControl.RequiredRevisionId,
                        StringComparison.Ordinal))
                {
                    throw new GoogleDocsBatchUpdateException(
                        HttpStatusCode.BadRequest,
                        "FAILED_PRECONDITION",
                        retryAfter: null,
                        RequestDeliveryState.Sent,
                        "Google Docs rejected a stale required revision.");
                }

                throw;
            }

            return new BatchUpdateDocumentResponse(ReadRevisionId(response));
        }

        internal static async Task<GoogleEndToEndContext?> TryCreateAsync(CancellationToken cancellationToken)
        {
            if (!string.Equals(Environment.GetEnvironmentVariable(EnableVariable), "1", StringComparison.Ordinal))
            {
                return null;
            }

            var credentialsPath = Environment.GetEnvironmentVariable(CredentialsPathVariable);
            if (string.IsNullOrWhiteSpace(credentialsPath))
            {
                return null;
            }

            var httpClient = new HttpClient();
            var options = new GooglePublisherOptions
            {
                AuthenticationMode = ParseAuthMode(),
                CredentialsPath = credentialsPath,
                TokenStorePath = Environment.GetEnvironmentVariable(TokenStorePathVariable) ?? string.Empty,
                FolderId = Environment.GetEnvironmentVariable(FolderIdVariable) ?? string.Empty,
            };
            var credentialProvider = GoogleCredentialProviderFactory.Create(options, httpClient);
            var documentId = await CreateOrCopyDocumentAsync(httpClient, credentialProvider, options, cancellationToken)
                .ConfigureAwait(false);
            return new GoogleEndToEndContext(httpClient, credentialProvider, documentId);
        }

        internal async Task<VerifiedPublishState> ResetAsync(
            string blockId,
            string text,
            CancellationToken cancellationToken)
        {
            var current = await GetDocumentBodyAsync(cancellationToken).ConfigureAwait(false);
            if (current.EndIndex > 2)
            {
                await SendAsync(
                    HttpMethod.Post,
                    $"https://docs.googleapis.com/v1/documents/{Uri.EscapeDataString(documentId)}:batchUpdate",
                    JsonSerializer.Serialize(new
                    {
                        requests = new object[]
                        {
                            new
                            {
                                deleteContentRange = new
                                {
                                    range = new { startIndex = 1, endIndex = current.EndIndex - 1 },
                                },
                            },
                        },
                    }),
                    cancellationToken).ConfigureAwait(false);
            }

            var candidate = CreateCandidate((blockId, text));
            await InsertManagedTextAsync(RenderManagedRegion(candidate), cancellationToken).ConfigureAwait(false);
            var snapshot = await reader.GetSnapshotAsync(candidate.Identity, cancellationToken).ConfigureAwait(false);
            var store = PublisherCompositionRoot.CreateVerifiedPublishStateStore(stateRoot);
            var evidence = new PublishApplicationVerification(
                candidate.Identity,
                new DiffEngine().CreatePlan(null, candidate),
                isLogicalPlanApplied: true,
                isReadbackVerified: true,
                snapshot.PublishFingerprint,
                snapshot.Blocks.Select(block => block.Identity),
                snapshot.Revision);
            var verified = new PublishResultVerifier().Verify(
                candidate,
                evidence.AppliedPlan,
                evidence);
            var state = new VerifiedPublishStatePromoter().Promote(null, verified);
            await store.SaveAsync(state, cancellationToken).ConfigureAwait(false);
            return (await store.LoadAsync(
                new PublishStateLoadRequest(
                    new PublishStateKey(candidate.Identity.PublicationId, candidate.Identity.DocumentId),
                    documentId),
                cancellationToken).ConfigureAwait(false))!;
        }

        internal PublishCandidate CreateCandidate(params (string Id, string Text)[] blocks)
        {
            var identity = new DocumentIdentity(
                "publication",
                "document",
                documentId,
                DocumentState.Active);
            return PublisherCompositionRoot.CreatePublishCandidateBuilder().Create(
                identity,
                new DocumentModel(blocks.Select(item =>
                    new DocumentBlock(ParagraphBlock.FromText(item.Text), item.Id))),
                new PublishCandidateBuildOptions(
                    "1.0.0",
                    "1.0",
                    "2",
                    [
                        new(PublishFingerprintSettingNames.MarkdownInlineMaxDepth, "8"),
                        new(PublishFingerprintSettingNames.MarkdownListIndentSize, "2"),
                        new(PublishFingerprintSettingNames.MarkdownListMaxDepth, "6"),
                        new(PublishFingerprintSettingNames.PublisherAllowImageUpscale, "false"),
                        new(PublishFingerprintSettingNames.PublisherImageMaxWidthPoints, "450"),
                    ]));
        }

        internal async Task<EndToEndResult> ApplyAndVerifyAsync(
            VerifiedPublishState baseline,
            PublishCandidate candidate,
            PhysicalUpdatePlan plan,
            CancellationToken cancellationToken)
        {
            var apply = await Executor.ExecuteAsync(plan, dryRun: false, cancellationToken).ConfigureAwait(false);
            return VerifyAfterApply(baseline, candidate, plan.LogicalPlan, apply);
        }

        internal async Task<PhysicalUpdatePlan> CreatePhysicalPlanAsync(
            VerifiedPublishState baseline,
            PublishCandidate candidate,
            CancellationToken cancellationToken)
        {
            var snapshot = await reader.GetSnapshotAsync(candidate.Identity, cancellationToken).ConfigureAwait(false);
            var logicalPlan = new DiffEngine().CreatePlan(baseline, candidate);
            markerlessReadbackCandidate = candidate;
            return new PhysicalUpdatePlanner().CreatePlan(baseline, candidate, logicalPlan, snapshot);
        }

        internal EndToEndResult VerifyAfterApply(
            VerifiedPublishState baseline,
            PublishCandidate candidate,
            DiffPlan logicalPlan,
            ApplyResult apply)
        {
            if (apply.Conflict || !apply.Applied && apply.Status != PhysicalUpdateExecutionStatus.NoChange)
            {
                return new EndToEndResult(apply, null!, false, false, apply.ErrorCode);
            }

            var readback = reader.GetSnapshotAsync(candidate.Identity, default).GetAwaiter().GetResult();
            if (apply.AppliedRevisionId is not null &&
                !string.Equals(apply.AppliedRevisionId, readback.Revision.RevisionId, StringComparison.Ordinal))
            {
                return new EndToEndResult(
                    apply,
                    readback,
                    false,
                    false,
                    UpdateErrorCodes.RevisionConflict);
            }

            try
            {
                var evidence = new PhysicalUpdateApplicationSnapshotVerifier()
                    .VerifyApplied(candidate, logicalPlan, readback);
                var verified = new PublishResultVerifier().Verify(candidate, logicalPlan, evidence);
                var promoted = new VerifiedPublishStatePromoter().Promote(baseline, verified);
                return new EndToEndResult(apply, readback, true, promoted is not null, null);
            }
            catch (PhysicalUpdateException exception)
            {
                return new EndToEndResult(apply, readback, false, false, exception.Code);
            }
            catch (StateLifecycleException exception)
            {
                return new EndToEndResult(apply, readback, false, false, exception.Code);
            }
        }

        internal async Task AppendOutsideManagedRegionAsync(string text, CancellationToken cancellationToken)
        {
            var current = await GetDocumentBodyAsync(cancellationToken).ConfigureAwait(false);
            await SendAsync(
                HttpMethod.Post,
                $"https://docs.googleapis.com/v1/documents/{Uri.EscapeDataString(documentId)}:batchUpdate",
                JsonSerializer.Serialize(new
                {
                    requests = new object[]
                    {
                        new
                        {
                            insertText = new
                            {
                                location = new { index = Math.Max(1, current.EndIndex - 1) },
                                text = "\n" + text,
                            },
                        },
                    },
                }),
                cancellationToken).ConfigureAwait(false);
        }

        internal async Task ReplaceManagedTextAsync(string managedText, CancellationToken cancellationToken)
        {
            var current = await reader.GetSnapshotAsync(
                new DocumentIdentity("publication", "document", documentId, DocumentState.Active),
                cancellationToken).ConfigureAwait(false);
            await SendAsync(
                HttpMethod.Post,
                $"https://docs.googleapis.com/v1/documents/{Uri.EscapeDataString(documentId)}:batchUpdate",
                JsonSerializer.Serialize(new
                {
                    requests = new object[]
                    {
                        new
                        {
                            deleteContentRange = new
                            {
                                range = new
                                {
                                    startIndex = current.ManagedRegion.StartIndex,
                                    endIndex = current.ManagedRegion.EndIndex,
                                },
                            },
                        },
                        new
                        {
                            insertText = new
                            {
                                location = new { index = current.ManagedRegion.StartIndex },
                                text = managedText,
                            },
                        },
                    },
                }),
                cancellationToken).ConfigureAwait(false);
        }

        private static GoogleAuthenticationMode ParseAuthMode() =>
            Enum.TryParse<GoogleAuthenticationMode>(
                Environment.GetEnvironmentVariable(AuthModeVariable),
                ignoreCase: true,
                out var mode)
                ? mode
                : GoogleAuthenticationMode.ServiceAccount;

        private static async Task<string> CreateOrCopyDocumentAsync(
            HttpClient httpClient,
            IGoogleCredentialProvider credentialProvider,
            GooglePublisherOptions options,
            CancellationToken cancellationToken)
        {
            var templateDocumentId = Environment.GetEnvironmentVariable(TemplateDocumentIdVariable);
            var helper = new DriveHelper(httpClient, credentialProvider);
            return string.IsNullOrWhiteSpace(templateDocumentId)
                ? await helper.CreateDocumentAsync(options.FolderId, cancellationToken).ConfigureAwait(false)
                : await helper.CopyDocumentAsync(templateDocumentId, options.FolderId, cancellationToken)
                    .ConfigureAwait(false);
        }

        private async Task InsertManagedTextAsync(string text, CancellationToken cancellationToken)
        {
            await SendAsync(
                HttpMethod.Post,
                $"https://docs.googleapis.com/v1/documents/{Uri.EscapeDataString(documentId)}:batchUpdate",
                JsonSerializer.Serialize(new
                {
                    requests = new object[]
                    {
                        new
                        {
                            insertText = new
                            {
                                location = new { index = 1 },
                                text,
                            },
                        },
                    },
                }),
                cancellationToken).ConfigureAwait(false);
        }

        private static string RenderManagedRegion(PublishCandidate candidate)
        {
            var builder = new StringBuilder();
            builder.AppendLine("<!-- vmf:managed-start fingerprint=" + candidate.Fingerprint.Value + " -->");
            foreach (var (block, payload) in candidate.Blocks.Zip(candidate.Document!.Blocks))
            {
                builder.AppendLine("<!-- vmf:block-id=" + block.ExplicitId + " -->");
                builder.AppendLine(PlainText(payload));
            }

            builder.AppendLine("<!-- vmf:managed-end -->");
            return builder.ToString();
        }

        private static string PlainText(DocumentBlock block) =>
            string.Concat(block.Content.Select(content => content switch
            {
                TextInline text => text.Text,
                _ => string.Empty,
            }));

        private async Task<DocumentBody> GetDocumentBodyAsync(CancellationToken cancellationToken)
        {
            var response = await SendAsync(
                HttpMethod.Get,
                $"https://docs.googleapis.com/v1/documents/{Uri.EscapeDataString(documentId)}",
                content: null,
                cancellationToken).ConfigureAwait(false);
            using var document = JsonDocument.Parse(response);
            var root = document.RootElement;
            var text = new StringBuilder();
            var endIndex = 1;
            foreach (var item in root.GetProperty("body").GetProperty("content").EnumerateArray())
            {
                if (item.TryGetProperty("endIndex", out var end) && end.TryGetInt32(out var value))
                {
                    endIndex = Math.Max(endIndex, value);
                }

                if (!item.TryGetProperty("paragraph", out var paragraph) ||
                    !paragraph.TryGetProperty("elements", out var elements))
                {
                    continue;
                }

                foreach (var element in elements.EnumerateArray())
                {
                    if (element.TryGetProperty("textRun", out var textRun) &&
                        textRun.TryGetProperty("content", out var content))
                    {
                        text.Append(content.GetString());
                    }
                }
            }

            return new DocumentBody(ReadRevisionId(response), endIndex, text.ToString());
        }

        private async Task DeleteDocumentAsync(CancellationToken cancellationToken)
        {
            try
            {
                await SendAsync(
                    HttpMethod.Delete,
                    $"https://www.googleapis.com/drive/v3/files/{Uri.EscapeDataString(documentId)}?supportsAllDrives=true",
                    content: null,
                    cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                // Cleanup is best-effort; test assertions must report the primary failure.
            }
        }

        private async Task<string> SendAsync(
            HttpMethod method,
            string requestUri,
            string? content,
            CancellationToken cancellationToken)
        {
            var credential = await credentialProvider.GetCredentialAsync(cancellationToken).ConfigureAwait(false);
            using var request = new HttpRequestMessage(method, requestUri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", credential.AccessToken);
            if (content is not null)
            {
                request.Content = new StringContent(content, Encoding.UTF8, "application/json");
            }

            using var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                throw new GoogleDocsBatchUpdateException(
                    response.StatusCode,
                    TryReadErrorReason(body) ?? $"HTTP_{(int)response.StatusCode}",
                    retryAfter: null,
                    RequestDeliveryState.Sent,
                    $"Google API request failed: HTTP {(int)response.StatusCode}.");
            }

            return body;
        }

        private static string ReadRevisionId(string responseBody)
        {
            using var document = JsonDocument.Parse(responseBody);
            return document.RootElement.TryGetProperty("revisionId", out var revision) &&
                revision.ValueKind == JsonValueKind.String
                    ? revision.GetString()!
                    : document.RootElement.TryGetProperty("writeControl", out var writeControl) &&
                    writeControl.TryGetProperty("requiredRevisionId", out var writeRevision) &&
                    writeRevision.ValueKind == JsonValueKind.String
                        ? writeRevision.GetString()!
                        : throw new InvalidOperationException("Google Docs response did not contain revisionId.");
        }

        private static string? TryReadErrorReason(string responseBody)
        {
            try
            {
                using var document = JsonDocument.Parse(responseBody);
                if (!document.RootElement.TryGetProperty("error", out var error))
                {
                    return null;
                }

                if (error.TryGetProperty("status", out var status) && status.ValueKind == JsonValueKind.String)
                {
                    return status.GetString();
                }

                return error.TryGetProperty("errors", out var errors) &&
                    errors.ValueKind == JsonValueKind.Array &&
                    errors.GetArrayLength() > 0 &&
                    errors[0].TryGetProperty("reason", out var reason) &&
                    reason.ValueKind == JsonValueKind.String
                        ? reason.GetString()
                        : null;
            }
            catch (JsonException)
            {
                return null;
            }
        }

        private sealed class GoogleDocsDocumentReader : IDocumentSnapshotReader
        {
            private readonly GoogleEndToEndContext owner;
            private readonly string documentId;

            internal GoogleDocsDocumentReader(GoogleEndToEndContext owner, string documentId)
            {
                this.owner = owner;
                this.documentId = documentId;
            }

            public async Task<ManagedDocumentSnapshot> GetSnapshotAsync(
                DocumentIdentity identity,
                CancellationToken cancellationToken)
            {
                var body = await owner.GetDocumentBodyAsync(cancellationToken).ConfigureAwait(false);
                var managedStart = body.Text.IndexOf("<!-- vmf:managed-start", StringComparison.Ordinal);
                var managedEnd = body.Text.IndexOf("<!-- vmf:managed-end -->", StringComparison.Ordinal);
                if (managedStart < 0 || managedEnd < managedStart)
                {
                    throw new InvalidOperationException("Managed region markers were not found.");
                }

                var markerEnd = body.Text.IndexOf("-->", managedStart, StringComparison.Ordinal);
                var fingerprint = ReadMarkerValue(body.Text[managedStart..markerEnd], "fingerprint=");
                var managedEndExclusive = managedEnd + "<!-- vmf:managed-end -->".Length + 1;
                var managedText = body.Text[managedStart..managedEndExclusive];
                var blocks = ReadBlocks(managedText, managedStart + 1).ToArray();
                if (blocks.Length == 0 && owner.markerlessReadbackCandidate is not null)
                {
                    blocks = ReadMarkerlessBlocks(
                        owner.markerlessReadbackCandidate,
                        managedText,
                        managedStart + 1).ToArray();
                }

                if (blocks.Length == 0)
                {
                    throw new InvalidOperationException("The managed region contains no managed blocks.");
                }

                var sequence = owner.GetRevisionSequence(body.RevisionId);
                return new ManagedDocumentSnapshot(
                    new DocumentIdentity(
                        identity.PublicationId,
                        identity.DocumentId,
                        documentId,
                        DocumentState.Active),
                    new DocumentRevision(body.RevisionId, sequence),
                    new DocumentTextRange(blocks[0].Range.StartIndex, blocks[^1].Range.EndIndex),
                    owner.markerlessReadbackCandidate is not null
                        ? owner.markerlessReadbackCandidate.Fingerprint.Value
                        : fingerprint,
                    blocks);
            }

            private static IEnumerable<ManagedBlockSnapshot> ReadBlocks(string managedText, int baseIndex)
            {
                var hashGenerator = new BlockContentHashGenerator();
                var searchIndex = 0;
                while (true)
                {
                    var markerStart = managedText.IndexOf("<!-- vmf:block-id=", searchIndex, StringComparison.Ordinal);
                    if (markerStart < 0)
                    {
                        yield break;
                    }

                    var markerEnd = managedText.IndexOf("-->", markerStart, StringComparison.Ordinal);
                    var blockId = ReadMarkerValue(managedText[markerStart..markerEnd], "block-id=");
                    var contentStart = markerEnd + 3;
                    if (contentStart < managedText.Length && managedText[contentStart] == '\n')
                    {
                        contentStart++;
                    }

                    var nextMarker = managedText.IndexOf("<!-- vmf:block-id=", contentStart, StringComparison.Ordinal);
                    var endMarker = managedText.IndexOf("<!-- vmf:managed-end -->", contentStart, StringComparison.Ordinal);
                    var contentEnd = nextMarker >= 0 ? nextMarker : endMarker;
                    var content = managedText[contentStart..contentEnd].TrimEnd('\r', '\n');
                    var block = new DocumentBlock(ParagraphBlock.FromText(content), blockId);
                    yield return new ManagedBlockSnapshot(
                        new BlockIdentity(blockId, null, hashGenerator.Generate(block)),
                        new DocumentTextRange(baseIndex + contentStart, baseIndex + contentEnd),
                        block);
                    searchIndex = contentEnd;
                }
            }

            private static IEnumerable<ManagedBlockSnapshot> ReadMarkerlessBlocks(
                PublishCandidate candidate,
                string managedText,
                int baseIndex)
            {
                var startMarkerEnd = managedText.IndexOf("-->", StringComparison.Ordinal);
                var endMarkerStart = managedText.IndexOf("<!-- vmf:managed-end -->", StringComparison.Ordinal);
                if (startMarkerEnd < 0 || endMarkerStart < startMarkerEnd)
                {
                    yield break;
                }

                var contentStart = startMarkerEnd + 3;
                if (contentStart < managedText.Length && managedText[contentStart] == '\n')
                {
                    contentStart++;
                }

                var content = managedText[contentStart..endMarkerStart].TrimEnd('\r', '\n');
                if (candidate.Blocks.Count != 1)
                {
                    throw new InvalidOperationException("Markerless readback supports only the single-block smoke fixture.");
                }

                var expected = candidate.Blocks[0];
                var block = new DocumentBlock(ParagraphBlock.FromText(content), expected.ExplicitId);
                yield return new ManagedBlockSnapshot(
                    new BlockIdentity(
                        expected.ExplicitId,
                        expected.GeneratedId,
                        new BlockContentHashGenerator().Generate(block)),
                    new DocumentTextRange(baseIndex + contentStart, baseIndex + endMarkerStart),
                    block);
            }

            private static string ReadMarkerValue(string marker, string prefix)
            {
                var start = marker.IndexOf(prefix, StringComparison.Ordinal);
                if (start < 0)
                {
                    throw new InvalidOperationException("Managed marker value was missing.");
                }

                start += prefix.Length;
                var end = marker.IndexOfAny([' ', '-'], start);
                return marker[start..(end < 0 ? marker.Length : end)];
            }
        }

        private long GetRevisionSequence(string revisionId)
        {
            if (revisionSequences.TryGetValue(revisionId, out var existing))
            {
                return existing;
            }

            sequence++;
            revisionSequences.Add(revisionId, sequence);
            return sequence;
        }
    }

    [CollectionDefinition("GoogleDocsEndToEndLive", DisableParallelization = true)]
    public sealed class GoogleDocsEndToEndLiveCollection
    {
    }

    private sealed record EndToEndResult(
        ApplyResult ApplyResult,
        ManagedDocumentSnapshot Readback,
        bool VerificationSuccess,
        bool VerifiedStateCommitAllowed,
        string? VerificationErrorCode);

    private sealed record DocumentBody(string RevisionId, int EndIndex, string Text);

    private static string Describe(ApplyResult result) => string.Join(
        ";",
        "status=" + result.Status,
        "error=" + (result.ErrorCode ?? "<none>"),
        "planned=" + result.PlannedOperationCount,
        "submitted=" + result.SubmittedRequestCount,
        "message=" + result.Message);

    private static string Describe(EndToEndResult result) => string.Join(
        ";",
        Describe(result.ApplyResult),
        "verification=" + result.VerificationSuccess,
        "commitAllowed=" + result.VerifiedStateCommitAllowed,
        "verificationError=" + (result.VerificationErrorCode ?? "<none>"));

    private sealed class DriveHelper
    {
        private readonly HttpClient httpClient;
        private readonly IGoogleCredentialProvider credentialProvider;

        internal DriveHelper(HttpClient httpClient, IGoogleCredentialProvider credentialProvider)
        {
            this.httpClient = httpClient;
            this.credentialProvider = credentialProvider;
        }

        internal async Task<string> CreateDocumentAsync(string folderId, CancellationToken cancellationToken) =>
            await SendDriveCreateAsync(new
            {
                name = "VMF Phase 3-7C E2E " + Guid.NewGuid().ToString("N"),
                mimeType = "application/vnd.google-apps.document",
                parents = string.IsNullOrWhiteSpace(folderId) ? null : new[] { folderId },
            }, cancellationToken).ConfigureAwait(false);

        internal async Task<string> CopyDocumentAsync(
            string templateDocumentId,
            string folderId,
            CancellationToken cancellationToken) =>
            await SendDriveCreateAsync(
                new
                {
                    name = "VMF Phase 3-7C E2E " + Guid.NewGuid().ToString("N"),
                    parents = string.IsNullOrWhiteSpace(folderId) ? null : new[] { folderId },
                },
                cancellationToken,
                $"https://www.googleapis.com/drive/v3/files/{Uri.EscapeDataString(templateDocumentId)}/copy?fields=id&supportsAllDrives=true")
                .ConfigureAwait(false);

        private async Task<string> SendDriveCreateAsync(
            object payload,
            CancellationToken cancellationToken,
            string requestUri = "https://www.googleapis.com/drive/v3/files?fields=id&supportsAllDrives=true")
        {
            var credential = await credentialProvider.GetCredentialAsync(cancellationToken).ConfigureAwait(false);
            using var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", credential.AccessToken);
            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            using var response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            using var document = JsonDocument.Parse(body);
            return document.RootElement.GetProperty("id").GetString()
                ?? throw new InvalidOperationException("Drive response did not contain file id.");
        }
    }
}
