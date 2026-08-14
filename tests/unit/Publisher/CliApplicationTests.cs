using System.Text.Json;
using System.Reflection;
using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;

namespace Vmf.Publisher.UnitTests;

public sealed class CliApplicationTests
{
    [Fact]
    public async Task RunAsync_HelpReturnsSuccessAndStructuredSummary()
    {
        using var capture = new ConsoleCapture();

        var exitCode = await CliApplication.RunAsync(["--help"], CancellationToken.None);

        Assert.Equal(0, exitCode);
        Assert.Contains("vmf-publisher publish", capture.Out, StringComparison.Ordinal);
        var summary = LastJsonLine(capture.Error);
        Assert.Equal("HELP", summary.GetProperty("code").GetString());
        Assert.Equal(0, summary.GetProperty("exitCode").GetInt32());
        Assert.Equal("help", summary.GetProperty("command").GetString());
        Assert.Equal("cli", summary.GetProperty("phase").GetString());
        Assert.Equal("summary", summary.GetProperty("operation").GetString());
        Assert.True(summary.TryGetProperty("sessionId", out var sessionId));
        Assert.StartsWith("pub-", sessionId.GetString(), StringComparison.Ordinal);
        AssertStructuredFields(JsonLines(capture.Error));

        var lines = JsonLines(capture.Error).ToArray();
        Assert.Equal("SESSION_STARTED", lines[0].GetProperty("code").GetString());
        Assert.Equal("Publisher diagnostic session started.", lines[0].GetProperty("message").GetString());
        Assert.Equal("COMMAND_STARTED", lines[1].GetProperty("code").GetString());
        Assert.Contains(lines, line => line.GetProperty("code").GetString() == "COMMAND_COMPLETED");
        AssertCommandStartedShape(lines, 1, true, "none");
        AssertInvocationShapeFieldsOnlyOnCommandStarted(lines);
        AssertSupportSummaryFieldsOnlyOnFinalFailureSummary(lines);
    }

    [Fact]
    public async Task RunAsync_VerifyCompilesMarkdownWithoutGoogleWrites()
    {
        using var capture = new ConsoleCapture();
        var path = Path.Combine(Path.GetTempPath(), $"vmf-publisher-cli-{Guid.NewGuid():N}.md");
        await File.WriteAllTextAsync(path, "# Title\n\nParagraph with **bold** text.\n");

        try
        {
            var exitCode = await CliApplication.RunAsync(["verify", path], CancellationToken.None);

            Assert.Equal(0, exitCode);
            var summary = LastJsonLine(capture.Error);
            Assert.Equal("VERIFY_SUCCEEDED", summary.GetProperty("code").GetString());
            Assert.Equal(0, summary.GetProperty("exitCode").GetInt32());
            Assert.Equal("verify", summary.GetProperty("command").GetString());
            Assert.Equal("verify", summary.GetProperty("phase").GetString());
            Assert.Equal("summary", summary.GetProperty("operation").GetString());
            Assert.Equal("verify", summary.GetProperty("lastPhase").GetString());
            Assert.Equal("report", summary.GetProperty("lastOperation").GetString());
            Assert.Equal("LOCAL_VERIFY_REPORT", summary.GetProperty("lastEventCode").GetString());
            AssertStructuredFields(JsonLines(capture.Error));
            AssertLastSafeOperationFieldsOnlyOnFinalSummary(JsonLines(capture.Error));
            AssertCommandStartedShape(JsonLines(capture.Error), 2, true, "verify-optional-markdown-path");
            AssertInvocationShapeFieldsOnlyOnCommandStarted(JsonLines(capture.Error));
            AssertSupportSummaryFieldsOnlyOnFinalFailureSummary(JsonLines(capture.Error));

            var report = LocalVerifyReport(capture.Error);
            Assert.Equal("localVerify", report.GetProperty("reportType").GetString());
            Assert.Equal(1, report.GetProperty("schemaVersion").GetInt32());
            Assert.Equal("PASS", report.GetProperty("overallResult").GetString());
            Assert.Equal(0, report.GetProperty("exitCode").GetInt32());
            Assert.Equal("VERIFY_SUCCEEDED", report.GetProperty("resultCode").GetString());
            Assert.Equal("Verification succeeded.", report.GetProperty("safeSummary").GetString());
            Assert.True(report.TryGetProperty("executedAtUtc", out _));
            AssertLocalVerifyCheckOrder(report);
            AssertLocalVerifyCheck(report, "configuration", "PASS", null);
            AssertLocalVerifyCheck(report, "markdownCompilation", "PASS", null);
            AssertLocalVerifyCheck(report, "localOnlyBoundary", "PASS", null);
            AssertLocalVerifyCheck(report, "liveE2E", "SKIPPED", null);
            AssertLocalVerifyCheck(report, "package", "SKIPPED", null);
            AssertLocalVerifyCheck(report, "release", "SKIPPED", null);
            AssertLocalVerifyCheck(report, "publication", "SKIPPED", null);
            AssertLocalVerifyMetadata(report);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task RunAsync_DiffReportsStepDelta()
    {
        using var capture = new ConsoleCapture();
        var before = Path.Combine(Path.GetTempPath(), $"vmf-publisher-before-{Guid.NewGuid():N}.md");
        var after = Path.Combine(Path.GetTempPath(), $"vmf-publisher-after-{Guid.NewGuid():N}.md");
        await File.WriteAllTextAsync(before, "# Title\n");
        await File.WriteAllTextAsync(after, "# Title\n\nParagraph.\n");

        try
        {
            var exitCode = await CliApplication.RunAsync(["diff", before, after], CancellationToken.None);

            Assert.Equal(0, exitCode);
            var diffSummary = JsonLines(capture.Error)
                .Single(line => line.GetProperty("code").GetString() == "DIFF_SUMMARY");
            Assert.Equal("diff", diffSummary.GetProperty("command").GetString());
            Assert.Equal("verification", diffSummary.GetProperty("phase").GetString());
            Assert.Equal("diff", diffSummary.GetProperty("operation").GetString());
            Assert.True(diffSummary.TryGetProperty("beforeStepCount", out _));
            Assert.True(diffSummary.TryGetProperty("afterStepCount", out _));
            Assert.True(diffSummary.TryGetProperty("stepDelta", out _));
        }
        finally
        {
            File.Delete(before);
            File.Delete(after);
        }
    }

    [Fact]
    public async Task RunAsync_PublishMissingPathReturnsConfigurationExitCode()
    {
        using var capture = new ConsoleCapture();
        var path = Path.Combine(Path.GetTempPath(), $"missing-{Guid.NewGuid():N}.md");

        var exitCode = await CliApplication.RunAsync(["publish", path], CancellationToken.None);

        Assert.Equal(1, exitCode);
        var summary = LastJsonLine(capture.Error);
        Assert.Equal("MARKDOWN_FILE_NOT_FOUND", summary.GetProperty("code").GetString());
        Assert.Equal("Input", summary.GetProperty("classification").GetString());
        Assert.Equal("publish", summary.GetProperty("command").GetString());
        Assert.Equal("publish", summary.GetProperty("phase").GetString());
        Assert.Equal("summary", summary.GetProperty("operation").GetString());
        Assert.Equal("publish", summary.GetProperty("lastPhase").GetString());
        Assert.Equal("loadSettings", summary.GetProperty("lastOperation").GetString());
        Assert.Equal("COMMAND_STARTED", summary.GetProperty("lastEventCode").GetString());
        Assert.Equal("Publisher input is invalid.", summary.GetProperty("message").GetString());
        Assert.False(summary.TryGetProperty("configurationCategory", out _));
        Assert.DoesNotContain(path, capture.Error, StringComparison.Ordinal);
        Assert.Contains(JsonLines(capture.Error), line =>
            line.GetProperty("code").GetString() == "COMMAND_FAILED");
    }

    [Fact]
    public async Task RunAsync_PublishUsageErrorReportsSafeInvocationShape()
    {
        using var capture = new ConsoleCapture();

        var exitCode = await CliApplication.RunAsync(["publish"], CancellationToken.None);

        Assert.Equal(2, exitCode);
        Assert.Contains("publish requires exactly one Markdown file path.", capture.Error, StringComparison.Ordinal);
        AssertCommandStartedShape(JsonLines(capture.Error), 1, true, "publish-markdown-path");
        AssertInvocationShapeFieldsOnlyOnCommandStarted(JsonLines(capture.Error));
    }

    [Theory]
    [InlineData("HELP", "None", 0)]
    [InlineData("USAGE_ERROR", "Usage", 2)]
    [InlineData("CONFIG_TIMEOUT_INVALID", "Configuration", 3)]
    [InlineData("MARKDOWN_EXPLICIT_ID_INVALID", "Input", 1)]
    [InlineData("PUBLISH_FILE_NOT_FOUND", "Input", 1)]
    [InlineData("UPDATE_READBACK_MISMATCH", "Verification", 4)]
    [InlineData("UPDATE_READBACK_FAILED", "Verification", 4)]
    [InlineData("UPDATE_REVISION_CONFLICT", "Verification", 4)]
    [InlineData("STATE_VERIFICATION_MISMATCH", "Verification", 4)]
    [InlineData("HTTP_503", "Transient", 75)]
    [InlineData("UNKNOWN_CODE", "Internal", 1)]
    public void ErrorMapping_StableCodesMapToClassificationAndExitCode(
        string code,
        string expectedClassification,
        int expectedExitCode)
    {
        var classification = Classify(code);

        Assert.Equal(expectedClassification, classification.ToString());
        Assert.Equal(expectedExitCode, ExitCodeFor(classification));
    }

    [Fact]
    public void ErrorMapping_BlankCodeFallsBackToInternal()
    {
        var classification = Classify("");

        Assert.Equal(ErrorClassification.Internal, classification);
        Assert.Equal(1, ExitCodeFor(classification));
        Assert.Equal("An internal Publisher error occurred.", SafeMessage(classification));
    }

    [Theory]
    [InlineData("CONFIG_TIMEOUT_INVALID", "cli")]
    [InlineData("CONFIG_INTEGER_INVALID", "cli")]
    [InlineData("CONFIG_CREDENTIALS_PATH_REQUIRED", "googleApi")]
    [InlineData("CONFIG_FOLDER_ID_REQUIRED", "googleApi")]
    [InlineData("CONFIG_TOKEN_STORE_PATH_REQUIRED", "googleApi")]
    [InlineData("CONFIG_AUTHENTICATION_MODE_INVALID", "googleApi")]
    [InlineData("CONFIG_IMAGE_MAX_WIDTH_INVALID", "publisher")]
    [InlineData("CONFIG_BOOLEAN_INVALID", "publisher")]
    [InlineData("CONFIG_NUMBER_INVALID", "publisher")]
    [InlineData("CONFIG_FUTURE_SAFE_CODE", "unknown")]
    public void ConfigurationCategory_MapsOnlyAllowListedCategories(string code, string expected)
    {
        Assert.Equal(expected, ConfigurationCategory(code));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("MARKDOWN_FILE_NOT_FOUND")]
    [InlineData("USAGE_ERROR")]
    [InlineData("PUBLISHER_ERROR")]
    public void ConfigurationCategory_NonConfigurationCodesReturnNull(string? code)
    {
        Assert.Null(ConfigurationCategory(code));
    }

    [Fact]
    public void StableErrorCodeValues_ArePreserved()
    {
        Assert.Equal("IMAGE_NOT_FOUND_AFTER_INSERT", PublishErrorCodes.ImageNotFoundAfterInsert);
        Assert.Equal("TABLE_DIMENSION_MISMATCH", PublishErrorCodes.TableDimensionMismatch);
        Assert.Equal("UPDATE_READBACK_MISMATCH", UpdateErrorCodes.ReadbackMismatch);
        Assert.Equal("UPDATE_REVISION_CONFLICT", UpdateErrorCodes.RevisionConflict);
        Assert.Equal("STATE_VERIFICATION_MISMATCH", StateErrorCodes.VerificationMismatch);
        Assert.Equal("STATE_DOCUMENT_IDENTITY_MISMATCH", StateErrorCodes.DocumentIdentityMismatch);
    }

    [Theory]
    [InlineData("Configuration", "Publisher configuration is invalid.")]
    [InlineData("Input", "Publisher input is invalid.")]
    [InlineData("Verification", "Publisher verification failed.")]
    [InlineData("Transient", "A transient external service error occurred.")]
    [InlineData("Canceled", "Operation was canceled.")]
    [InlineData("Internal", "An internal Publisher error occurred.")]
    public void SafeMessage_UsesClassificationFixedMessage(string classificationName, string expected)
    {
        var classification = Enum.Parse<ErrorClassification>(classificationName);

        var message = SafeMessage(classification);

        Assert.Equal(expected, message);
        Assert.DoesNotContain("https://", message, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("token", message, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("secret", message, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(@"C:\", message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RunAsync_CanceledReturnsExit130WithoutRawExceptionMessage()
    {
        using var capture = new ConsoleCapture();
        using var cancellation = new CancellationTokenSource();
        var path = Path.Combine(Path.GetTempPath(), $"vmf-publisher-canceled-{Guid.NewGuid():N}.md");
        await File.WriteAllTextAsync(path, "# Title\n\nParagraph.\n");
        cancellation.Cancel();

        try
        {
            var exitCode = await CliApplication.RunAsync(["verify", path], cancellation.Token);

            Assert.Equal(130, exitCode);
            var summary = LastJsonLine(capture.Error);
            Assert.Equal("CANCELED", summary.GetProperty("code").GetString());
            Assert.Equal("Canceled", summary.GetProperty("classification").GetString());
            Assert.Equal("Operation was canceled.", summary.GetProperty("message").GetString());
            Assert.Equal("verify", summary.GetProperty("lastPhase").GetString());
            Assert.Equal("compile", summary.GetProperty("lastOperation").GetString());
            Assert.Equal("COMMAND_STARTED", summary.GetProperty("lastEventCode").GetString());
            Assert.DoesNotContain(
                "OperationCanceledException",
                summary.GetProperty("message").GetString(),
                StringComparison.Ordinal);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task PublishService_DoesNotExposeRawUnexpectedExceptionMessage()
    {
        var sensitiveMessage = "boom https://example.test/body C:\\secret\\file.md token=abc secret=value";
        var service = CreatePublishService(_ => throw new InvalidOperationException(sensitiveMessage));

        var result = await service.PublishAsync(new PublishRequest("input.md"), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("PUBLISH_FAILED", result.Error?.Code);
        Assert.Equal("Publication failed.", result.Error?.Message);
        Assert.DoesNotContain("https://", result.Error?.Message, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(@"C:\secret", result.Error?.Message, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("token", result.Error?.Message, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("secret", result.Error?.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PublishService_DoesNotExposeRawStableExceptionMessage()
    {
        var sensitiveMessage = "readback mismatch at https://docs.example.test C:\\tmp\\doc token secret";
        var service = CreatePublishService(_ =>
            throw new PhysicalUpdateException(UpdateErrorCodes.ReadbackMismatch, sensitiveMessage));

        var result = await service.PublishAsync(new PublishRequest("input.md"), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(UpdateErrorCodes.ReadbackMismatch, result.Error?.Code);
        Assert.Equal("Publisher verification failed.", result.Error?.Message);
        Assert.DoesNotContain("https://", result.Error?.Message, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(@"C:\tmp", result.Error?.Message, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("token", result.Error?.Message, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("secret", result.Error?.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PublishService_RethrowsOperationCanceledException()
    {
        var service = CreatePublishService(_ => throw new OperationCanceledException("secret cancel message"));

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => service.PublishAsync(new PublishRequest("input.md"), CancellationToken.None));
    }

    [Fact]
    public async Task RunAsync_DryRunReportsPlannerPhaseAndPlanOperation()
    {
        using var capture = new ConsoleCapture();
        var path = Path.Combine(Path.GetTempPath(), $"vmf-publisher-dry-run-{Guid.NewGuid():N}.md");
        await File.WriteAllTextAsync(path, "# Title\n\nParagraph.\n");

        try
        {
            var exitCode = await CliApplication.RunAsync(["dry-run", path], CancellationToken.None);

            Assert.Equal(0, exitCode);
            var plan = JsonLines(capture.Error)
                .Single(line => line.GetProperty("code").GetString() == "DRY_RUN_PLAN");
            Assert.Equal("dry-run", plan.GetProperty("command").GetString());
            Assert.Equal("planner", plan.GetProperty("phase").GetString());
            Assert.Equal("plan", plan.GetProperty("operation").GetString());
            Assert.Equal("local-dry-run", plan.GetProperty("mode").GetString());
            Assert.False(plan.TryGetProperty("contractVersion", out _));
            Assert.True(plan.TryGetProperty("stepCount", out _));
            AssertDryRunPlanSummary(plan, stepCount: 1, operationCount: 3);
            Assert.Equal(1, plan.GetProperty("headingOperationCount").GetInt32());
            Assert.Equal(0, plan.GetProperty("listOperationCount").GetInt32());
            Assert.Equal(0, plan.GetProperty("tableStepCount").GetInt32());
            Assert.Equal(0, plan.GetProperty("imageStepCount").GetInt32());
            AssertDryRunBoundary(plan);
            Assert.DoesNotContain(path, capture.Error, StringComparison.Ordinal);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task RunAsync_DryRunEmitsFlatSuccessSummaryContract()
    {
        using var capture = new ConsoleCapture();
        var path = Path.Combine(Path.GetTempPath(), $"vmf-publisher-dry-run-{Guid.NewGuid():N}.md");
        await File.WriteAllTextAsync(path, "# Title\n\nParagraph.\n");

        try
        {
            var exitCode = await CliApplication.RunAsync(["dry-run", path], CancellationToken.None);

            Assert.Equal(0, exitCode);
            var lines = JsonLines(capture.Error).ToArray();
            var planIndex = Array.FindIndex(lines, line => line.GetProperty("code").GetString() == "DRY_RUN_PLAN");
            var dryRunSummaryIndex = Array.FindIndex(lines, line => line.GetProperty("code").GetString() == "DRY_RUN_SUMMARY");
            var finalSummaryIndex = Array.FindLastIndex(lines, line => line.GetProperty("code").GetString() == "DRY_RUN_SUCCEEDED");
            Assert.True(planIndex >= 0);
            Assert.True(dryRunSummaryIndex > planIndex);
            Assert.True(finalSummaryIndex > dryRunSummaryIndex);

            var summary = lines[dryRunSummaryIndex];
            Assert.Equal("dry-run", summary.GetProperty("command").GetString());
            Assert.Equal("planner", summary.GetProperty("phase").GetString());
            Assert.Equal("summary", summary.GetProperty("operation").GetString());
            Assert.Equal(1, summary.GetProperty("contractVersion").GetInt32());
            Assert.Equal("succeeded", summary.GetProperty("planningResult").GetString());
            AssertDryRunSummaryContract(summary, stepCount: 1, operationCount: 3);
            Assert.False(summary.TryGetProperty("safePlanSummary", out _));
            Assert.False(summary.TryGetProperty("failureBoundary", out _));
            Assert.False(summary.TryGetProperty("exitCode", out _));
            AssertOnlyFlatDryRunSummaryFields(summary);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task RunAsync_DryRunBoundaryDoesNotClaimPublicationVerificationOrClearance()
    {
        using var capture = new ConsoleCapture();
        var path = Path.Combine(Path.GetTempPath(), $"vmf-publisher-dry-run-{Guid.NewGuid():N}.md");
        await File.WriteAllTextAsync(path, "# Title\n\nParagraph.\n");

        try
        {
            var exitCode = await CliApplication.RunAsync(["dry-run", path], CancellationToken.None);

            Assert.Equal(0, exitCode);
            var plan = JsonLines(capture.Error)
                .Single(line => line.GetProperty("code").GetString() == "DRY_RUN_PLAN");
            AssertDryRunBoundary(plan);
            Assert.Equal("DRY_RUN_SUCCEEDED", LastJsonLine(capture.Error).GetProperty("code").GetString());
            Assert.DoesNotContain("publication success", capture.Error, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("google verification", capture.Error, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("verified state saved", capture.Error, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("release clearance", capture.Error, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("vendor clearance", capture.Error, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task RunAsync_DryRunReportsRepresentativeSafePlanSummary()
    {
        using var capture = new ConsoleCapture();
        var path = Path.Combine(Path.GetTempPath(), $"vmf-publisher-dry-run-{Guid.NewGuid():N}.md");
        await File.WriteAllTextAsync(
            path,
            """
            # Sensitive Title token-secret

            Paragraph with https://private.example.test/doc and Authorization: Bearer value.

            - Item one
            - Item two

            | Name | Value |
            | --- | --- |
            | token | secret |

            ```text
            credential=value
            ```

            > quoted private content
            """);

        try
        {
            var exitCode = await CliApplication.RunAsync(["dry-run", path], CancellationToken.None);

            Assert.Equal(0, exitCode);
            var plan = JsonLines(capture.Error)
                .Single(line => line.GetProperty("code").GetString() == "DRY_RUN_PLAN");
            AssertDryRunPlanSummary(plan, stepCount: 3, operationCount: 11);
            Assert.Equal(2, plan.GetProperty("batchUpdateStepCount").GetInt32());
            Assert.Equal(1, plan.GetProperty("tableStepCount").GetInt32());
            Assert.Equal(0, plan.GetProperty("imageStepCount").GetInt32());
            Assert.Equal(1, plan.GetProperty("headingOperationCount").GetInt32());
            Assert.Equal(1, plan.GetProperty("listOperationCount").GetInt32());
            Assert.Equal(1, plan.GetProperty("codeBlockOperationCount").GetInt32());
            Assert.Equal(1, plan.GetProperty("quoteOperationCount").GetInt32());
            Assert.Equal("succeeded", plan.GetProperty("markdownCompilation").GetString());
            Assert.Equal("local-only", plan.GetProperty("planningEvidence").GetString());
            Assert.DoesNotContain(path, capture.Error, StringComparison.Ordinal);
            Assert.DoesNotContain("Sensitive Title", capture.Error, StringComparison.Ordinal);
            Assert.DoesNotContain("private.example.test", capture.Error, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("Authorization", capture.Error, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("credential=value", capture.Error, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("token-secret", capture.Error, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task RunAsync_DryRunEmptyInputReportsZeroPlanSummary()
    {
        using var capture = new ConsoleCapture();
        var path = Path.Combine(Path.GetTempPath(), $"vmf-publisher-dry-run-empty-{Guid.NewGuid():N}.md");
        await File.WriteAllTextAsync(path, string.Empty);

        try
        {
            var exitCode = await CliApplication.RunAsync(["dry-run", path], CancellationToken.None);

            Assert.Equal(0, exitCode);
            var plan = JsonLines(capture.Error)
                .Single(line => line.GetProperty("code").GetString() == "DRY_RUN_PLAN");
            AssertDryRunPlanSummary(plan, stepCount: 0, operationCount: 0);
            Assert.Equal(0, plan.GetProperty("batchUpdateStepCount").GetInt32());
            Assert.Equal(0, plan.GetProperty("tableStepCount").GetInt32());
            Assert.Equal(0, plan.GetProperty("imageStepCount").GetInt32());
            Assert.Equal(0, plan.GetProperty("insertTextOperationCount").GetInt32());
            Assert.Equal(0, plan.GetProperty("headingOperationCount").GetInt32());
            Assert.Equal(0, plan.GetProperty("listOperationCount").GetInt32());
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task RunAsync_DryRunInvalidInputDoesNotEmitPlanBoundaryOrSensitivePath()
    {
        using var capture = new ConsoleCapture();
        var path = Path.Combine(Path.GetTempPath(), $"vmf-publisher-token-secret-{Guid.NewGuid():N}.md");

        var exitCode = await CliApplication.RunAsync(["dry-run", path], CancellationToken.None);

        Assert.Equal(1, exitCode);
        Assert.DoesNotContain("DRY_RUN_PLAN", capture.Error, StringComparison.Ordinal);
        Assert.DoesNotContain("DRY_RUN_SUMMARY", capture.Error, StringComparison.Ordinal);
        Assert.DoesNotContain("local-dry-run", capture.Error, StringComparison.Ordinal);
        Assert.DoesNotContain(path, capture.Error, StringComparison.Ordinal);
        Assert.DoesNotContain("token-secret", capture.Error, StringComparison.OrdinalIgnoreCase);
        var summary = LastJsonLine(capture.Error);
        Assert.Equal("input", summary.GetProperty("failureBoundary").GetString());
    }

    [Fact]
    public async Task RunAsync_DryRunUsageFailureReportsFailureBoundaryOnlyOnFinalSummary()
    {
        using var capture = new ConsoleCapture();

        var exitCode = await CliApplication.RunAsync(["dry-run"], CancellationToken.None);

        Assert.Equal(2, exitCode);
        var summary = LastJsonLine(capture.Error);
        Assert.Equal("USAGE_ERROR", summary.GetProperty("code").GetString());
        Assert.Equal("usage", summary.GetProperty("failureBoundary").GetString());
        Assert.DoesNotContain("DRY_RUN_SUMMARY", capture.Error, StringComparison.Ordinal);
        AssertFailureBoundaryFieldsOnlyOnDryRunFailureSummary(JsonLines(capture.Error));
        AssertSupportSummaryFieldsOnlyOnFinalFailureSummary(JsonLines(capture.Error));
    }

    [Fact]
    public async Task RunAsync_UnknownCommandReportsSafeUsageWithoutEchoingCommand()
    {
        using var capture = new ConsoleCapture();
        var sensitiveCommand = "token-secret-command";

        var exitCode = await CliApplication.RunAsync([sensitiveCommand], CancellationToken.None);

        Assert.Equal(2, exitCode);
        var summary = LastJsonLine(capture.Error);
        Assert.Equal("USAGE_ERROR", summary.GetProperty("code").GetString());
        Assert.Equal("unknown", summary.GetProperty("command").GetString());
        Assert.Equal("cli", summary.GetProperty("phase").GetString());
        Assert.Equal("summary", summary.GetProperty("operation").GetString());
        AssertCommandStartedShape(JsonLines(capture.Error), 1, false, "none");
        AssertInvocationShapeFieldsOnlyOnCommandStarted(JsonLines(capture.Error));
        Assert.DoesNotContain(sensitiveCommand, capture.Error, StringComparison.Ordinal);
    }

    [Fact]
    public async Task RunAsync_SafeInvocationShapeDoesNotEchoSensitiveArguments()
    {
        using var capture = new ConsoleCapture();
        var sensitiveArgument = @"C:\Users\biz\private-token-secret.md";

        var exitCode = await CliApplication.RunAsync(
            ["diff", sensitiveArgument, "https://private.example.test/doc"],
            CancellationToken.None);

        Assert.Equal(1, exitCode);
        AssertCommandStartedShape(JsonLines(capture.Error), 3, true, "diff-before-after");
        Assert.DoesNotContain(sensitiveArgument, capture.Error, StringComparison.Ordinal);
        Assert.DoesNotContain("private.example.test", capture.Error, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("token-secret", capture.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RunAsync_InvalidEnvironmentValueDoesNotLogSensitiveValue()
    {
        using var capture = new ConsoleCapture();
        using var environment = new EnvironmentScope(
            "VMF_PUBLISHER_OPERATION_TIMEOUT_SECONDS",
            @"C:\Users\biz\secret-token.txt");

        var exitCode = await CliApplication.RunAsync(["verify"], CancellationToken.None);

        Assert.Equal(3, exitCode);
        var summary = LastJsonLine(capture.Error);
        Assert.Equal("CONFIG_INTEGER_INVALID", summary.GetProperty("code").GetString());
        Assert.Equal("Publisher configuration is invalid.", summary.GetProperty("message").GetString());
        Assert.Equal("cli", summary.GetProperty("configurationCategory").GetString());
        Assert.Equal("verify", summary.GetProperty("lastPhase").GetString());
        Assert.Equal("report", summary.GetProperty("lastOperation").GetString());
        Assert.Equal("LOCAL_VERIFY_REPORT", summary.GetProperty("lastEventCode").GetString());
        AssertCommandStartedShape(JsonLines(capture.Error), 1, true, "verify-optional-markdown-path");
        AssertInvocationShapeFieldsOnlyOnCommandStarted(JsonLines(capture.Error));
        AssertConfigurationCategoryFieldsOnlyOnConfigurationSummary(JsonLines(capture.Error));
        Assert.DoesNotContain("secret-token", capture.Error, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(@"C:\Users\biz", capture.Error, StringComparison.OrdinalIgnoreCase);

        var report = LocalVerifyReport(capture.Error);
        Assert.Equal("FAIL", report.GetProperty("overallResult").GetString());
        Assert.Equal(3, report.GetProperty("exitCode").GetInt32());
        Assert.Equal("CONFIG_INTEGER_INVALID", report.GetProperty("resultCode").GetString());
        Assert.Equal("Publisher configuration is invalid.", report.GetProperty("safeSummary").GetString());
        AssertLocalVerifyCheckOrder(report);
        AssertLocalVerifyCheck(report, "configuration", "FAIL", "CONFIG_INTEGER_INVALID");
        AssertLocalVerifyCheck(report, "markdownCompilation", "SKIPPED", null);
        AssertLocalVerifyCheck(report, "liveE2E", "SKIPPED", null);
        AssertLocalVerifyMetadata(report);
        Assert.DoesNotContain("secret-token", report.GetRawText(), StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(@"C:\Users\biz", report.GetRawText(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RunAsync_VerifyWithoutMarkdownReportsCompilationSkipped()
    {
        using var capture = new ConsoleCapture();

        var exitCode = await CliApplication.RunAsync(["verify"], CancellationToken.None);

        Assert.Equal(0, exitCode);
        var report = LocalVerifyReport(capture.Error);
        Assert.Equal("PASS", report.GetProperty("overallResult").GetString());
        Assert.Equal(0, report.GetProperty("exitCode").GetInt32());
        AssertLocalVerifyCheckOrder(report);
        AssertLocalVerifyCheck(report, "configuration", "PASS", null);
        AssertLocalVerifyCheck(report, "markdownCompilation", "SKIPPED", null);
        AssertLocalVerifyCheck(report, "liveE2E", "SKIPPED", null);
        AssertLocalVerifyCheck(report, "package", "SKIPPED", null);
        AssertLocalVerifyCheck(report, "release", "SKIPPED", null);
        AssertLocalVerifyCheck(report, "publication", "SKIPPED", null);
    }

    [Fact]
    public async Task RunAsync_VerifyMissingMarkdownReportsSafeFailure()
    {
        using var capture = new ConsoleCapture();
        var path = Path.Combine(Path.GetTempPath(), $"missing-token-secret-{Guid.NewGuid():N}.md");

        var exitCode = await CliApplication.RunAsync(["verify", path], CancellationToken.None);

        Assert.Equal(1, exitCode);
        var report = LocalVerifyReport(capture.Error);
        Assert.Equal("FAIL", report.GetProperty("overallResult").GetString());
        Assert.Equal(1, report.GetProperty("exitCode").GetInt32());
        Assert.Equal("MARKDOWN_FILE_NOT_FOUND", report.GetProperty("resultCode").GetString());
        Assert.Equal("Publisher input is invalid.", report.GetProperty("safeSummary").GetString());
        AssertLocalVerifyCheck(report, "configuration", "PASS", null);
        AssertLocalVerifyCheck(report, "markdownCompilation", "FAIL", "MARKDOWN_FILE_NOT_FOUND");
        AssertLocalVerifyCheck(report, "liveE2E", "SKIPPED", null);
        Assert.DoesNotContain(path, capture.Error, StringComparison.Ordinal);
        Assert.DoesNotContain("token-secret", report.GetRawText(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void SafeMessage_ForTransientExternalExceptionReturnsClassifiedMessage()
    {
        var method = typeof(CliApplication).GetMethod(
            "SafeMessage",
            BindingFlags.NonPublic | BindingFlags.Static);

        var message = Assert.IsType<string>(method?.Invoke(
            null,
            [ErrorClassification.Transient]));

        Assert.Equal("A transient external service error occurred.", message);
        Assert.DoesNotContain("https://", message, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("token", message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void StructuredPublisherLogger_WarningUsesExecutorContextAndSafeMessage()
    {
        using var capture = new ConsoleCapture();
        var logger = new StructuredPublisherLogger("pub-test", "publish");

        logger.Warning(
            PublishErrorCodes.ImageAltTextUpdateFailed,
            "Google Docs image insertion cannot set alt text; alt text remains in the publish model only.");

        var warning = LastJsonLine(capture.Error);
        Assert.Equal("warning", warning.GetProperty("level").GetString());
        Assert.Equal("publish", warning.GetProperty("command").GetString());
        Assert.Equal("executor", warning.GetProperty("phase").GetString());
        Assert.Equal("insertImage", warning.GetProperty("operation").GetString());
        Assert.Equal(
            PublishErrorCodes.ImageAltTextUpdateFailed,
            warning.GetProperty("code").GetString());
        Assert.DoesNotContain("https://", capture.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void StructuredPublisherLogger_SummaryIncludesLastSafeOperationWithoutSensitiveValues()
    {
        using var capture = new ConsoleCapture();
        var logger = new StructuredPublisherLogger("pub-test", "publish");
        const string sensitive = "https://private.example.test/body C:\\secret\\file.md token=abc Authorization: Bearer value";

        logger.SetContext("publish", "execute");
        logger.Info("PUBLISH_EXECUTE_STARTED", "Publish execution started.", "publish", "execute");
        logger.Summary(
            CliResult.Failure(
                1,
                "PUBLISHER_ERROR",
                "An internal Publisher error occurred.",
                ErrorClassification.Internal,
                "InvalidOperationException"),
            TimeSpan.FromMilliseconds(25));

        var summary = LastJsonLine(capture.Error);
        Assert.Equal("publish", summary.GetProperty("lastPhase").GetString());
        Assert.Equal("execute", summary.GetProperty("lastOperation").GetString());
        Assert.Equal("PUBLISH_EXECUTE_STARTED", summary.GetProperty("lastEventCode").GetString());
        Assert.DoesNotContain(sensitive, capture.Error, StringComparison.Ordinal);
        Assert.DoesNotContain("private.example.test", capture.Error, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(@"C:\secret", capture.Error, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Authorization", capture.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void StructuredPublisherLogger_FailureSummaryIncludesSafeRetryDiagnosticsWhenKnown()
    {
        using var capture = new ConsoleCapture();
        var logger = new StructuredPublisherLogger("pub-test", "publish");

        logger.Summary(
            CliResult.Failure(
                75,
                "TRANSIENT_FAILURE",
                "A transient external service error occurred.",
                ErrorClassification.Transient,
                retryDiagnostics: new RetryDiagnostics(3, 5, true)),
            TimeSpan.FromMilliseconds(25));

        var summary = LastJsonLine(capture.Error);
        Assert.Equal(3, summary.GetProperty("attemptCount").GetInt32());
        Assert.Equal(5, summary.GetProperty("maxAttempts").GetInt32());
        Assert.True(summary.GetProperty("retryable").GetBoolean());
        var supportSummary = summary.GetProperty("SUPPORT_SUMMARY");
        Assert.Equal("TRANSIENT_FAILURE", supportSummary.GetProperty("resultCode").GetString());
        Assert.Equal("Transient", supportSummary.GetProperty("classification").GetString());
        Assert.Equal(75, supportSummary.GetProperty("exitCode").GetInt32());
        Assert.Equal("publish", supportSummary.GetProperty("command").GetString());
        Assert.Equal("publish", supportSummary.GetProperty("phase").GetString());
        Assert.Equal("summary", supportSummary.GetProperty("operation").GetString());
        Assert.Equal(
            "A transient external service error occurred.",
            supportSummary.GetProperty("safeMessage").GetString());
        Assert.Equal(3, supportSummary.GetProperty("attemptCount").GetInt32());
        Assert.Equal(5, supportSummary.GetProperty("maxAttempts").GetInt32());
        Assert.True(supportSummary.GetProperty("retryable").GetBoolean());
        Assert.Equal("not-applicable", supportSummary.GetProperty("readbackStatus").GetString());
        Assert.Equal(
            "managed-document-readback-only",
            supportSummary.GetProperty("readbackEvidenceBoundary").GetString());
        Assert.False(summary.TryGetProperty("deliveryState", out _));
        Assert.False(summary.TryGetProperty("httpStatus", out _));
        Assert.False(supportSummary.TryGetProperty("deliveryState", out _));
        Assert.False(supportSummary.TryGetProperty("httpStatus", out _));
        Assert.DoesNotContain("https://", capture.Error, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(@"C:\secret", capture.Error, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Authorization", capture.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void StructuredPublisherLogger_FailureSummaryOmitsRetryDiagnosticsWhenUnknown()
    {
        using var capture = new ConsoleCapture();
        var logger = new StructuredPublisherLogger("pub-test", "publish");

        logger.Summary(
            CliResult.Failure(
                1,
                "PUBLISHER_ERROR",
                "An internal Publisher error occurred.",
                ErrorClassification.Internal),
            TimeSpan.FromMilliseconds(25));

        var summary = LastJsonLine(capture.Error);
        Assert.False(summary.TryGetProperty("attemptCount", out _));
        Assert.False(summary.TryGetProperty("maxAttempts", out _));
        Assert.False(summary.TryGetProperty("retryable", out _));
    }

    [Theory]
    [InlineData("USAGE_ERROR", "Usage", "usage")]
    [InlineData("CONFIG_TIMEOUT_INVALID", "Configuration", "configuration")]
    [InlineData("MARKDOWN_FILE_NOT_FOUND", "Input", "input")]
    [InlineData("PUBLISHER_ERROR", "Internal", "internal")]
    [InlineData("CANCELED", "Canceled", "cancellation")]
    [InlineData("HTTP_503", "Transient", "unknown")]
    public void StructuredPublisherLogger_DryRunFailureSummaryReportsAllowListedBoundary(
        string code,
        string classificationName,
        string expectedBoundary)
    {
        using var capture = new ConsoleCapture();
        var logger = new StructuredPublisherLogger("pub-test", "dry-run");
        var classification = Enum.Parse<ErrorClassification>(classificationName);

        logger.Summary(
            CliResult.Failure(
                ExitCodeFor(classification),
                code,
                SafeMessage(classification),
                classification),
            TimeSpan.FromMilliseconds(25));

        var summary = LastJsonLine(capture.Error);
        Assert.Equal(expectedBoundary, summary.GetProperty("failureBoundary").GetString());
        Assert.DoesNotContain("https://", capture.Error, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(@"C:\secret", capture.Error, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Authorization", capture.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void StructuredPublisherLogger_DryRunCompileFailureReportsCompileBoundary()
    {
        using var capture = new ConsoleCapture();
        var logger = new StructuredPublisherLogger("pub-test", "dry-run");

        logger.SetContext("planner", "compile");
        logger.Summary(
            CliResult.Failure(
                1,
                "MARKDOWN_PARSE_FAILED",
                "Publisher input is invalid.",
                ErrorClassification.Input),
            TimeSpan.FromMilliseconds(25));

        var summary = LastJsonLine(capture.Error);
        Assert.Equal("compile", summary.GetProperty("failureBoundary").GetString());
    }

    [Fact]
    public void StructuredPublisherLogger_OmitsFailureBoundaryOutsideDryRunFailureSummary()
    {
        using var capture = new ConsoleCapture();
        var publishLogger = new StructuredPublisherLogger("pub-test", "publish");
        var dryRunLogger = new StructuredPublisherLogger("pub-test", "dry-run");

        publishLogger.Summary(
            CliResult.Failure(
                1,
                "MARKDOWN_FILE_NOT_FOUND",
                "Publisher input is invalid.",
                ErrorClassification.Input),
            TimeSpan.FromMilliseconds(25));
        dryRunLogger.Summary(
            CliResult.Success("DRY_RUN_SUCCEEDED", "Dry run completed."),
            TimeSpan.FromMilliseconds(25));

        foreach (var line in JsonLines(capture.Error))
        {
            Assert.False(line.TryGetProperty("failureBoundary", out _));
        }
    }

    [Theory]
    [InlineData(UpdateErrorCodes.ReadbackFailed, "failed", "post-apply-readback")]
    [InlineData(UpdateErrorCodes.ReadbackMismatch, "mismatch", "post-apply-readback")]
    [InlineData(UpdateErrorCodes.ManagedRegionMismatch, "mismatch", "post-apply-readback")]
    [InlineData(UpdateErrorCodes.RevisionConflict, "revision-conflict", "pre-apply-read")]
    public void StructuredPublisherLogger_SummaryReportsReadbackStatusWithoutClearance(
        string code,
        string expectedStatus,
        string expectedPhase)
    {
        using var capture = new ConsoleCapture();
        var logger = new StructuredPublisherLogger("pub-test", "publish");

        logger.Summary(
            CliResult.Failure(
                4,
                code,
                "Publisher verification failed.",
                ErrorClassification.Verification),
            TimeSpan.FromMilliseconds(25));

        var summary = LastJsonLine(capture.Error);
        Assert.Equal(expectedStatus, summary.GetProperty("readbackStatus").GetString());
        Assert.Equal(expectedPhase, summary.GetProperty("readbackPhase").GetString());
        Assert.Equal("managed-document-readback-only", summary.GetProperty("readbackEvidenceBoundary").GetString());
        Assert.False(summary.GetProperty("readbackVerified").GetBoolean());
        Assert.False(summary.GetProperty("verifiedStateSaved").GetBoolean());
        Assert.False(summary.GetProperty("publicationAuthorized").GetBoolean());
        Assert.False(summary.GetProperty("releaseClearance").GetBoolean());
        Assert.False(summary.GetProperty("packageApproval").GetBoolean());
        Assert.False(summary.GetProperty("avastSafetyCertification").GetBoolean());
        Assert.False(summary.GetProperty("vendorClearance").GetBoolean());
        Assert.DoesNotContain("https://", capture.Error, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(@"C:\secret", capture.Error, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Authorization", capture.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void StructuredPublisherLogger_SummaryPreservesDocumentCompatibilityFields()
    {
        using var capture = new ConsoleCapture();
        var logger = new StructuredPublisherLogger("pub-test", "publish");

        logger.Summary(
            CliResult.Success(
                "PUBLISH_SUCCEEDED",
                "Publication succeeded.",
                "document-id",
                "https://docs.google.com/document/d/document-id/edit"),
            TimeSpan.FromMilliseconds(25));

        var summary = LastJsonLine(capture.Error);
        Assert.Equal("PUBLISH_SUCCEEDED", summary.GetProperty("code").GetString());
        Assert.Equal("publish", summary.GetProperty("command").GetString());
        Assert.Equal("publish", summary.GetProperty("phase").GetString());
        Assert.Equal("summary", summary.GetProperty("operation").GetString());
        Assert.Equal("document-id", summary.GetProperty("documentId").GetString());
        Assert.False(summary.TryGetProperty("configurationCategory", out _));
        Assert.False(summary.TryGetProperty("attemptCount", out _));
        Assert.False(summary.TryGetProperty("retryable", out _));
        Assert.False(summary.TryGetProperty("SUPPORT_SUMMARY", out _));
        Assert.Equal(
            "https://docs.google.com/document/d/document-id/edit",
            summary.GetProperty("documentUrl").GetString());
    }

    private static JsonElement LastJsonLine(string text)
    {
        var line = text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Last(value => value.StartsWith('{'));
        using var document = JsonDocument.Parse(line);
        return document.RootElement.Clone();
    }

    private static JsonElement LocalVerifyReport(string text)
    {
        var line = JsonLines(text)
            .Single(value => value.GetProperty("code").GetString() == "LOCAL_VERIFY_REPORT");
        return line;
    }

    private static IEnumerable<JsonElement> JsonLines(string text)
    {
        foreach (var line in text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Where(value => value.StartsWith('{')))
        {
            using var document = JsonDocument.Parse(line);
            yield return document.RootElement.Clone();
        }
    }

    private static void AssertStructuredFields(IEnumerable<JsonElement> lines)
    {
        foreach (var line in lines)
        {
            Assert.True(line.TryGetProperty("timestampUtc", out _));
            Assert.True(line.TryGetProperty("level", out _));
            Assert.True(line.TryGetProperty("sessionId", out _));
            Assert.True(line.TryGetProperty("command", out _));
            Assert.True(line.TryGetProperty("phase", out _));
            Assert.True(line.TryGetProperty("operation", out _));
            Assert.True(line.TryGetProperty("code", out _));
            Assert.True(line.TryGetProperty("message", out _));
        }
    }

    private static void AssertDryRunBoundary(JsonElement plan)
    {
        Assert.Equal("local-dry-run", plan.GetProperty("mode").GetString());
        Assert.Equal("not-attempted", plan.GetProperty("googleDocsMutation").GetString());
        Assert.Equal("not-attempted", plan.GetProperty("googleDriveMutation").GetString());
        Assert.Equal("not-attempted", plan.GetProperty("oauthOperation").GetString());
        Assert.Equal("not-attempted", plan.GetProperty("tokenStoreOperation").GetString());
        Assert.False(plan.GetProperty("physicalUpdatePlanApplied").GetBoolean());
        Assert.Equal("not-attempted", plan.GetProperty("readbackStatus").GetString());
        Assert.Equal("post-apply-readback", plan.GetProperty("readbackPhase").GetString());
        Assert.Equal("managed-document-readback-only", plan.GetProperty("readbackEvidenceBoundary").GetString());
        Assert.False(plan.GetProperty("readbackVerified").GetBoolean());
        Assert.False(plan.GetProperty("verifiedStateSaved").GetBoolean());
        Assert.False(plan.GetProperty("publicationAuthorized").GetBoolean());
        Assert.False(plan.GetProperty("releaseClearance").GetBoolean());
        Assert.False(plan.GetProperty("packageApproval").GetBoolean());
        Assert.False(plan.GetProperty("avastSafetyCertification").GetBoolean());
        Assert.False(plan.GetProperty("vendorClearance").GetBoolean());
    }

    private static void AssertDryRunPlanSummary(JsonElement plan, int stepCount, int operationCount)
    {
        Assert.Equal(stepCount, plan.GetProperty("stepCount").GetInt32());
        Assert.Equal(operationCount, plan.GetProperty("operationCount").GetInt32());
        Assert.True(plan.TryGetProperty("batchUpdateStepCount", out _));
        Assert.True(plan.TryGetProperty("tableStepCount", out _));
        Assert.True(plan.TryGetProperty("imageStepCount", out _));
        Assert.True(plan.TryGetProperty("insertTextOperationCount", out _));
        Assert.True(plan.TryGetProperty("headingOperationCount", out _));
        Assert.True(plan.TryGetProperty("listOperationCount", out _));
        Assert.True(plan.TryGetProperty("textStyleOperationCount", out _));
        Assert.True(plan.TryGetProperty("paragraphAlignmentOperationCount", out _));
        Assert.True(plan.TryGetProperty("codeBlockOperationCount", out _));
        Assert.True(plan.TryGetProperty("quoteOperationCount", out _));
        Assert.Equal("succeeded", plan.GetProperty("markdownCompilation").GetString());
        Assert.Equal("local-only", plan.GetProperty("planningEvidence").GetString());

        var safeSummary = plan.GetProperty("safePlanSummary").GetString();
        Assert.NotNull(safeSummary);
        Assert.Contains($"compiled {stepCount} publish step(s)", safeSummary, StringComparison.Ordinal);
        Assert.Contains($"{operationCount} operation(s)", safeSummary, StringComparison.Ordinal);
        Assert.Contains("Google Docs/Drive mutation", safeSummary, StringComparison.Ordinal);
        Assert.Contains("were not attempted", safeSummary, StringComparison.Ordinal);
    }

    private static void AssertDryRunSummaryContract(JsonElement summary, int stepCount, int operationCount)
    {
        Assert.Equal("local-dry-run", summary.GetProperty("mode").GetString());
        Assert.Equal("succeeded", summary.GetProperty("markdownCompilation").GetString());
        Assert.Equal("local-only", summary.GetProperty("planningEvidence").GetString());
        Assert.Equal(stepCount, summary.GetProperty("stepCount").GetInt32());
        Assert.Equal(operationCount, summary.GetProperty("operationCount").GetInt32());
        Assert.True(summary.TryGetProperty("batchUpdateStepCount", out _));
        Assert.True(summary.TryGetProperty("tableStepCount", out _));
        Assert.True(summary.TryGetProperty("imageStepCount", out _));
        Assert.True(summary.TryGetProperty("insertTextOperationCount", out _));
        Assert.True(summary.TryGetProperty("headingOperationCount", out _));
        Assert.True(summary.TryGetProperty("listOperationCount", out _));
        Assert.True(summary.TryGetProperty("textStyleOperationCount", out _));
        Assert.True(summary.TryGetProperty("paragraphAlignmentOperationCount", out _));
        Assert.True(summary.TryGetProperty("codeBlockOperationCount", out _));
        Assert.True(summary.TryGetProperty("quoteOperationCount", out _));
        Assert.Equal("not-attempted", summary.GetProperty("googleDocsMutation").GetString());
        Assert.Equal("not-attempted", summary.GetProperty("googleDriveMutation").GetString());
        Assert.Equal("not-attempted", summary.GetProperty("oauthOperation").GetString());
        Assert.Equal("not-attempted", summary.GetProperty("tokenStoreOperation").GetString());
        Assert.False(summary.GetProperty("physicalUpdatePlanApplied").GetBoolean());
        Assert.Equal("not-attempted", summary.GetProperty("readbackStatus").GetString());
        Assert.False(summary.GetProperty("readbackVerified").GetBoolean());
        Assert.False(summary.GetProperty("verifiedStateSaved").GetBoolean());
        Assert.False(summary.GetProperty("publicationAuthorized").GetBoolean());
        Assert.False(summary.GetProperty("releaseClearance").GetBoolean());
        Assert.False(summary.GetProperty("packageApproval").GetBoolean());
        Assert.False(summary.GetProperty("vendorClearance").GetBoolean());
        Assert.False(summary.GetProperty("avastSafetyCertification").GetBoolean());
    }

    private static void AssertOnlyFlatDryRunSummaryFields(JsonElement summary)
    {
        foreach (var property in summary.EnumerateObject())
        {
            Assert.NotEqual(JsonValueKind.Object, property.Value.ValueKind);
            Assert.NotEqual(JsonValueKind.Array, property.Value.ValueKind);
        }
    }

    private static void AssertCommandStartedShape(
        IEnumerable<JsonElement> lines,
        int argumentCount,
        bool recognizedCommand,
        string expectedArgumentShape)
    {
        var commandStarted = lines.Single(line => line.GetProperty("code").GetString() == "COMMAND_STARTED");
        Assert.Equal(argumentCount, commandStarted.GetProperty("argumentCount").GetInt32());
        Assert.Equal(recognizedCommand, commandStarted.GetProperty("recognizedCommand").GetBoolean());
        Assert.Equal(expectedArgumentShape, commandStarted.GetProperty("expectedArgumentShape").GetString());
    }

    private static void AssertInvocationShapeFieldsOnlyOnCommandStarted(IEnumerable<JsonElement> lines)
    {
        foreach (var line in lines)
        {
            var isCommandStarted = line.GetProperty("code").GetString() == "COMMAND_STARTED";
            Assert.Equal(isCommandStarted, line.TryGetProperty("argumentCount", out _));
            Assert.Equal(isCommandStarted, line.TryGetProperty("recognizedCommand", out _));
            Assert.Equal(isCommandStarted, line.TryGetProperty("expectedArgumentShape", out _));
        }
    }

    private static void AssertLastSafeOperationFieldsOnlyOnFinalSummary(IEnumerable<JsonElement> lines)
    {
        foreach (var line in lines)
        {
            var operation = line.GetProperty("operation").GetString();
            var isFinalSummary = operation == "summary" &&
                line.TryGetProperty("exitCode", out _) &&
                line.TryGetProperty("classification", out _);
            Assert.Equal(isFinalSummary, line.TryGetProperty("lastPhase", out _));
            Assert.Equal(isFinalSummary, line.TryGetProperty("lastOperation", out _));
            Assert.Equal(isFinalSummary, line.TryGetProperty("lastEventCode", out _));
        }
    }

    private static void AssertConfigurationCategoryFieldsOnlyOnConfigurationSummary(IEnumerable<JsonElement> lines)
    {
        foreach (var line in lines)
        {
            var isConfigurationSummary = line.GetProperty("operation").GetString() == "summary" &&
                line.TryGetProperty("classification", out var classification) &&
                classification.GetString() == "Configuration";
            Assert.Equal(isConfigurationSummary, line.TryGetProperty("configurationCategory", out _));
        }
    }

    private static void AssertFailureBoundaryFieldsOnlyOnDryRunFailureSummary(IEnumerable<JsonElement> lines)
    {
        foreach (var line in lines)
        {
            var isDryRunFailureSummary = line.GetProperty("command").GetString() == "dry-run" &&
                line.GetProperty("operation").GetString() == "summary" &&
                line.TryGetProperty("exitCode", out var exitCode) &&
                exitCode.GetInt32() != 0;
            Assert.Equal(isDryRunFailureSummary, line.TryGetProperty("failureBoundary", out _));
        }
    }

    private static void AssertSupportSummaryFieldsOnlyOnFinalFailureSummary(IEnumerable<JsonElement> lines)
    {
        foreach (var line in lines)
        {
            var isFinalFailureSummary = line.GetProperty("operation").GetString() == "summary" &&
                line.TryGetProperty("exitCode", out var exitCode) &&
                exitCode.GetInt32() != 0;
            Assert.Equal(isFinalFailureSummary, line.TryGetProperty("SUPPORT_SUMMARY", out _));
        }
    }

    private static void AssertLocalVerifyCheckOrder(JsonElement report)
    {
        var names = report.GetProperty("checks")
            .EnumerateArray()
            .Select(check => check.GetProperty("name").GetString())
            .ToArray();

        Assert.Equal(
            [
                "configuration",
                "markdownCompilation",
                "localOnlyBoundary",
                "liveE2E",
                "package",
                "release",
                "publication",
            ],
            names);
    }

    private static void AssertLocalVerifyCheck(
        JsonElement report,
        string name,
        string expectedStatus,
        string? expectedFailureCode)
    {
        var check = report.GetProperty("checks")
            .EnumerateArray()
            .Single(value => value.GetProperty("name").GetString() == name);

        Assert.Equal(expectedStatus, check.GetProperty("status").GetString());
        Assert.True(check.TryGetProperty("safeSummary", out _));
        if (expectedFailureCode is null)
        {
            Assert.False(check.TryGetProperty("failureCode", out _));
        }
        else
        {
            Assert.Equal(expectedFailureCode, check.GetProperty("failureCode").GetString());
        }
    }

    private static void AssertLocalVerifyMetadata(JsonElement report)
    {
        var configuration = report.GetProperty("configuration");
        Assert.False(configuration.GetProperty("googlePublishSettingsRequired").GetBoolean());
        Assert.True(configuration.GetProperty("localOnly").GetBoolean());
        Assert.False(configuration.GetProperty("liveE2EIncludedInSuccessCriteria").GetBoolean());
        Assert.False(configuration.GetProperty("packageIncludedInSuccessCriteria").GetBoolean());
        Assert.False(configuration.GetProperty("releaseIncludedInSuccessCriteria").GetBoolean());
        Assert.False(configuration.GetProperty("publicationIncludedInSuccessCriteria").GetBoolean());

        var environment = report.GetProperty("environment");
        Assert.False(string.IsNullOrWhiteSpace(environment.GetProperty("dotNetRuntime").GetString()));
        Assert.False(string.IsNullOrWhiteSpace(environment.GetProperty("osDescription").GetString()));
        Assert.False(string.IsNullOrWhiteSpace(environment.GetProperty("osArchitecture").GetString()));
        Assert.False(string.IsNullOrWhiteSpace(environment.GetProperty("processArchitecture").GetString()));

        var constraints = report.GetProperty("constraints");
        Assert.True(constraints.GetProperty("localOnly").GetBoolean());
        Assert.False(constraints.GetProperty("liveE2EIncludedInSuccessCriteria").GetBoolean());
        Assert.False(constraints.GetProperty("packageIncludedInSuccessCriteria").GetBoolean());
        Assert.False(constraints.GetProperty("releaseIncludedInSuccessCriteria").GetBoolean());
        Assert.False(constraints.GetProperty("publicationIncludedInSuccessCriteria").GetBoolean());
        Assert.Equal("SKIPPED", constraints.GetProperty("liveE2EStatus").GetString());
        Assert.Equal("SKIPPED", constraints.GetProperty("googleDocsDriveMutationStatus").GetString());
        Assert.Equal("SKIPPED", constraints.GetProperty("packageStatus").GetString());
        Assert.Equal("SKIPPED", constraints.GetProperty("releaseStatus").GetString());
        Assert.Equal("SKIPPED", constraints.GetProperty("publicationStatus").GetString());
    }

    private static ErrorClassification Classify(string? code)
    {
        var method = typeof(CliApplication).GetMethod(
            "Classify",
            BindingFlags.NonPublic | BindingFlags.Static);

        return Assert.IsType<ErrorClassification>(method?.Invoke(null, [code]));
    }

    private static int ExitCodeFor(ErrorClassification classification)
    {
        var method = typeof(CliApplication).GetMethod(
            "ExitCodeFor",
            BindingFlags.NonPublic | BindingFlags.Static);

        return Assert.IsType<int>(method?.Invoke(null, [classification]));
    }

    private static string SafeMessage(ErrorClassification classification)
    {
        var method = typeof(CliApplication).GetMethod(
            "SafeMessage",
            BindingFlags.NonPublic | BindingFlags.Static);

        return Assert.IsType<string>(method?.Invoke(null, [classification]));
    }

    private static string? ConfigurationCategory(string? code)
    {
        var method = typeof(CliApplication).GetMethod(
            "ConfigurationCategory",
            BindingFlags.NonPublic | BindingFlags.Static);

        return method?.Invoke(null, [code]) as string;
    }

    private static PublishService CreatePublishService(Func<CompiledDocument, PublishedDocument> publish)
    {
        return new PublishService(
            new StubLoader(),
            new StubParser(),
            new StubCompiler(),
            new StubPublisher(publish));
    }

    private sealed class EnvironmentScope : IDisposable
    {
        private readonly string name;
        private readonly string? originalValue;

        internal EnvironmentScope(string name, string value)
        {
            this.name = name;
            originalValue = Environment.GetEnvironmentVariable(name);
            Environment.SetEnvironmentVariable(name, value);
        }

        public void Dispose() => Environment.SetEnvironmentVariable(name, originalValue);
    }

    private sealed class StubLoader : IMarkdownDocumentLoader
    {
        public Task<string> LoadAsync(string path, CancellationToken cancellationToken) =>
            Task.FromResult("# Title");
    }

    private sealed class StubParser : IMarkdownParser
    {
        public DocumentModel Parse(string markdown) => new([]);
    }

    private sealed class StubCompiler : IDocumentCompiler
    {
        public CompiledDocument Compile(DocumentModel document, string title) => new(title, []);
    }

    private sealed class StubPublisher(Func<CompiledDocument, PublishedDocument> publish) : IGoogleDocsPublisher
    {
        public Task<PublishedDocument> PublishAsync(CompiledDocument document, CancellationToken cancellationToken) =>
            Task.FromResult(publish(document));
    }

    private sealed class ConsoleCapture : IDisposable
    {
        private readonly TextWriter originalOut = Console.Out;
        private readonly TextWriter originalError = Console.Error;
        private readonly StringWriter outWriter = new();
        private readonly StringWriter errorWriter = new();

        internal ConsoleCapture()
        {
            Console.SetOut(outWriter);
            Console.SetError(errorWriter);
        }

        internal string Out => outWriter.ToString();

        internal string Error => errorWriter.ToString();

        public void Dispose()
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
            outWriter.Dispose();
            errorWriter.Dispose();
        }
    }
}
