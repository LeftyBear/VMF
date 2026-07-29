using System.Text.Json;

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
        Assert.True(summary.TryGetProperty("sessionId", out var sessionId));
        Assert.StartsWith("pub-", sessionId.GetString(), StringComparison.Ordinal);
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
