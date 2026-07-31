using System.Text.Json;
using System.Reflection;

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
            AssertStructuredFields(JsonLines(capture.Error));
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

        Assert.Equal(3, exitCode);
        var summary = LastJsonLine(capture.Error);
        Assert.Equal("MARKDOWN_FILE_NOT_FOUND", summary.GetProperty("code").GetString());
        Assert.Equal("Configuration", summary.GetProperty("classification").GetString());
        Assert.Equal("publish", summary.GetProperty("command").GetString());
        Assert.Equal("publish", summary.GetProperty("phase").GetString());
        Assert.Equal("summary", summary.GetProperty("operation").GetString());
        Assert.Equal("Markdown file was not found.", summary.GetProperty("message").GetString());
        Assert.DoesNotContain(path, capture.Error, StringComparison.Ordinal);
        Assert.Contains(JsonLines(capture.Error), line =>
            line.GetProperty("code").GetString() == "COMMAND_FAILED");
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
            Assert.True(plan.TryGetProperty("stepCount", out _));
            Assert.DoesNotContain(path, capture.Error, StringComparison.Ordinal);
        }
        finally
        {
            File.Delete(path);
        }
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
        Assert.DoesNotContain(sensitiveCommand, capture.Error, StringComparison.Ordinal);
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
        Assert.Equal("Invalid positive integer setting value.", summary.GetProperty("message").GetString());
        Assert.DoesNotContain("secret-token", capture.Error, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(@"C:\Users\biz", capture.Error, StringComparison.OrdinalIgnoreCase);
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
            Vmf.Publisher.Application.PublishErrorCodes.ImageAltTextUpdateFailed,
            "Google Docs image insertion cannot set alt text; alt text remains in the publish model only.");

        var warning = LastJsonLine(capture.Error);
        Assert.Equal("warning", warning.GetProperty("level").GetString());
        Assert.Equal("publish", warning.GetProperty("command").GetString());
        Assert.Equal("executor", warning.GetProperty("phase").GetString());
        Assert.Equal("insertImage", warning.GetProperty("operation").GetString());
        Assert.Equal(
            Vmf.Publisher.Application.PublishErrorCodes.ImageAltTextUpdateFailed,
            warning.GetProperty("code").GetString());
        Assert.DoesNotContain("https://", capture.Error, StringComparison.OrdinalIgnoreCase);
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
