using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text.Json;
using Vmf.Publisher.Application;
using Vmf.Publisher.Domain;
using Vmf.Publisher.Infrastructure;
using Vmf.Publisher.Infrastructure.Google;

using var cancellationSource = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    cancellationSource.Cancel();
};

return await CliApplication.RunAsync(args, cancellationSource.Token).ConfigureAwait(false);

internal static class CliApplication
{
    private const int ExitSuccess = 0;
    private const int ExitPublishFailed = 1;
    private const int ExitUsage = 2;
    private const int ExitConfiguration = 3;
    private const int ExitVerification = 4;
    private const int ExitTransient = 75;
    private const int ExitCanceled = 130;

    private const string HelpText =
        """
        VMF Publisher

        Usage:
          vmf-publisher publish <markdown-file>
          vmf-publisher verify [markdown-file]
          vmf-publisher diff <before-markdown-file> <after-markdown-file>
          vmf-publisher dry-run <markdown-file>
          vmf-publisher --help

        Commands:
          publish <markdown-file>          Publish a Markdown file to Google Docs.
          verify [markdown-file]           Validate configuration and optionally compile Markdown locally.
          diff <before> <after>            Compare compiled local Markdown summaries.
          dry-run <markdown-file>          Compile Markdown and print a publish summary without Google writes.

        Options:
          -h, --help                       Show this help.
        """;

    internal static async Task<int> RunAsync(string[] arguments, CancellationToken cancellationToken)
    {
        var sessionId = CreateSessionId();
        var stopwatch = Stopwatch.StartNew();
        var logger = new StructuredPublisherLogger(sessionId);
        logger.Info("SESSION_STARTED", "Publish session started.");

        try
        {
            var result = await RunCommandAsync(arguments, logger, cancellationToken).ConfigureAwait(false);
            logger.Summary(result, stopwatch.Elapsed);
            return result.ExitCode;
        }
        catch (OperationCanceledException)
        {
            var result = CliResult.Failure(
                ExitCanceled,
                "CANCELED",
                "Publication was canceled.",
                ErrorClassification.Canceled);
            logger.Summary(result, stopwatch.Elapsed);
            return result.ExitCode;
        }
        catch (CliConfigurationException exception)
        {
            var result = CliResult.Failure(
                ExitConfiguration,
                exception.Code,
                exception.Message,
                ErrorClassification.Configuration);
            logger.Summary(result, stopwatch.Elapsed);
            return result.ExitCode;
        }
        catch (Exception exception) when (IsTransient(exception))
        {
            var result = CliResult.Failure(
                ExitTransient,
                "TRANSIENT_ERROR",
                SafeMessage(exception),
                ErrorClassification.Transient);
            logger.Summary(result, stopwatch.Elapsed);
            return result.ExitCode;
        }
        catch (Exception exception)
        {
            var result = CliResult.Failure(
                ExitPublishFailed,
                "PUBLISHER_ERROR",
                SafeMessage(exception),
                ErrorClassification.Internal);
            logger.Summary(result, stopwatch.Elapsed);
            return result.ExitCode;
        }
    }

    private static async Task<CliResult> RunCommandAsync(
        string[] arguments,
        StructuredPublisherLogger logger,
        CancellationToken cancellationToken)
    {
        if (arguments.Length == 0 || IsHelp(arguments[0]))
        {
            Console.WriteLine(HelpText);
            return CliResult.Success("HELP", "Help displayed.");
        }

        var command = arguments[0].ToLowerInvariant();
        return command switch
        {
            "publish" => await PublishAsync(arguments, logger, cancellationToken).ConfigureAwait(false),
            "verify" => await VerifyAsync(arguments, logger, cancellationToken).ConfigureAwait(false),
            "diff" => await DiffAsync(arguments, logger, cancellationToken).ConfigureAwait(false),
            "dry-run" => await DryRunAsync(arguments, logger, cancellationToken).ConfigureAwait(false),
            _ => UsageFailure($"Unknown command: {arguments[0]}"),
        };
    }

    private static async Task<CliResult> PublishAsync(
        string[] arguments,
        StructuredPublisherLogger logger,
        CancellationToken cancellationToken)
    {
        if (arguments.Length == 2 && IsHelp(arguments[1]))
        {
            Console.WriteLine(HelpText);
            return CliResult.Success("HELP", "Help displayed.");
        }

        if (arguments.Length != 2)
        {
            return UsageFailure("publish requires exactly one Markdown file path.");
        }

        var settings = LoadSettings(requireGooglePublishSettings: true);
        ValidateMarkdownPath(arguments[1]);
        using var timeoutSource = CreateTimeoutSource(settings.Cli.OperationTimeoutSeconds, cancellationToken);
        var publishService = CreatePublishService(settings, logger);
        var result = await publishService.PublishAsync(
            new PublishRequest(arguments[1]),
            timeoutSource.Token).ConfigureAwait(false);
        if (!result.IsSuccess)
        {
            var classification = Classify(result.Error?.Code);
            return CliResult.Failure(
                classification == ErrorClassification.Transient ? ExitTransient : ExitPublishFailed,
                result.Error?.Code ?? "PUBLISH_FAILED",
                result.Error?.Message ?? "Publication failed.",
                classification);
        }

        Console.WriteLine("Google Drive API: success");
        Console.WriteLine("Google Docs API: success");
        Console.WriteLine($"Document ID: {result.DocumentId}");
        Console.WriteLine($"Document URL: {result.DocumentUrl}");
        return CliResult.Success("PUBLISH_SUCCEEDED", "Publication succeeded.", result.DocumentId, result.DocumentUrl);
    }

    private static async Task<CliResult> VerifyAsync(
        string[] arguments,
        StructuredPublisherLogger logger,
        CancellationToken cancellationToken)
    {
        if (arguments.Length > 2)
        {
            return UsageFailure("verify accepts zero or one Markdown file path.");
        }

        var settings = LoadSettings(requireGooglePublishSettings: false);
        if (arguments.Length == 2)
        {
            ValidateMarkdownPath(arguments[1]);
            await CompileAsync(arguments[1], settings.Publisher, cancellationToken).ConfigureAwait(false);
        }

        logger.Info("CONFIGURATION_VALID", "Configuration validation succeeded.");
        return CliResult.Success("VERIFY_SUCCEEDED", "Verification succeeded.");
    }

    private static async Task<CliResult> DryRunAsync(
        string[] arguments,
        StructuredPublisherLogger logger,
        CancellationToken cancellationToken)
    {
        if (arguments.Length != 2)
        {
            return UsageFailure("dry-run requires exactly one Markdown file path.");
        }

        var settings = LoadSettings(requireGooglePublishSettings: false);
        ValidateMarkdownPath(arguments[1]);
        var compiled = await CompileAsync(arguments[1], settings.Publisher, cancellationToken).ConfigureAwait(false);
        logger.PublishPlan("DRY_RUN_PLAN", compiled);
        return CliResult.Success("DRY_RUN_SUCCEEDED", "Dry run completed.");
    }

    private static async Task<CliResult> DiffAsync(
        string[] arguments,
        StructuredPublisherLogger logger,
        CancellationToken cancellationToken)
    {
        if (arguments.Length != 3)
        {
            return UsageFailure("diff requires before and after Markdown file paths.");
        }

        var settings = LoadSettings(requireGooglePublishSettings: false);
        ValidateMarkdownPath(arguments[1]);
        ValidateMarkdownPath(arguments[2]);
        var before = await CompileAsync(arguments[1], settings.Publisher, cancellationToken).ConfigureAwait(false);
        var after = await CompileAsync(arguments[2], settings.Publisher, cancellationToken).ConfigureAwait(false);
        logger.DiffSummary(before, after);
        return CliResult.Success("DIFF_SUCCEEDED", "Diff completed.");
    }

    private static CliResult UsageFailure(string message)
    {
        Console.Error.WriteLine(message);
        Console.Error.WriteLine(HelpText);
        return CliResult.Failure(ExitUsage, "USAGE_ERROR", message, ErrorClassification.Usage);
    }

    private static IPublishService CreatePublishService(PublisherCliSettings settings, IPublisherLogger logger)
    {
        var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(settings.Cli.HttpTimeoutSeconds) };
        var imageHttpClient = new HttpClient(new HttpClientHandler
        {
            AllowAutoRedirect = false,
        })
        {
            Timeout = TimeSpan.FromSeconds(settings.Cli.HttpTimeoutSeconds),
        };
        var credentialProvider = GoogleCredentialProviderFactory.Create(settings.Google, httpClient);
        var requestMapper = new GoogleDocsRequestMapper();
        var serviceFactory = new GoogleServiceFactory(credentialProvider, requestMapper, httpClient);
        var services = CreateLocalServices(settings.Publisher, imageHttpClient);
        ITemporaryImageHost temporaryImageHost = new GoogleDriveTemporaryImageHost(
            credentialProvider,
            httpClient,
            settings.Google,
            settings.Publisher,
            logger);
        IPublishPlanExecutor publishPlanExecutor = new PublishPlanExecutor(
            serviceFactory.CreateDocsClient(),
            services.InlineRenderer,
            temporaryImageHost,
            logger);
        var googlePublisher = new GoogleDocsPublisher(serviceFactory, settings.Google, publishPlanExecutor);
        return new PublishService(
            new MarkdownFileDocumentLoader(),
            services.Parser,
            services.Compiler,
            googlePublisher,
            services.ImageSourceResolver,
            services.ImageMetadataReader,
            services.ImageSizeCalculator);
    }

    private static async Task<CompiledDocument> CompileAsync(
        string markdownPath,
        PublisherOptions options,
        CancellationToken cancellationToken)
    {
        using var imageHttpClient = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false });
        var services = CreateLocalServices(options, imageHttpClient);
        var markdown = await new MarkdownFileDocumentLoader()
            .LoadAsync(markdownPath, cancellationToken).ConfigureAwait(false);
        var model = await PrepareImagesAsync(
            services.Parser.Parse(markdown),
            markdownPath,
            services,
            cancellationToken).ConfigureAwait(false);
        return services.Compiler.Compile(model, Path.GetFileNameWithoutExtension(markdownPath));
    }

    private static async Task<DocumentModel> PrepareImagesAsync(
        DocumentModel model,
        string markdownPath,
        LocalPublisherServices services,
        CancellationToken cancellationToken)
    {
        if (!model.Blocks.Any(block => block.Kind == DocumentBlockKind.Image))
        {
            return model;
        }

        var blocks = new List<DocumentBlock>(model.Blocks.Count);
        foreach (var block in model.Blocks)
        {
            if (block.Kind != DocumentBlockKind.Image)
            {
                blocks.Add(block);
                continue;
            }

            var image = block.Image
                ?? throw new InvalidOperationException("An image block requires image content.");
            var resolved = await services.ImageSourceResolver.ResolveAsync(
                image.Source,
                markdownPath,
                cancellationToken).ConfigureAwait(false);
            var metadata = await services.ImageMetadataReader.ReadAsync(resolved, cancellationToken)
                .ConfigureAwait(false);
            var size = services.ImageSizeCalculator.Calculate(metadata);
            blocks.Add(new DocumentBlock(
                new ImageBlock(
                    image.AltText,
                    metadata.Source,
                    size,
                    metadata.ContentHash,
                    image.StableId),
                block.ExplicitId));
        }

        return new DocumentModel(blocks);
    }

    private static LocalPublisherServices CreateLocalServices(
        PublisherOptions options,
        HttpClient imageHttpClient)
    {
        var inlineParser = new MarkdownInlineParser(new MarkdownInlineParserOptions
        {
            MaxInlineDepth = MarkdownInlineParserOptions.DefaultMaxInlineDepth,
        });
        var markdownListParser = new MarkdownListParser(new MarkdownListParserOptions
        {
            ListIndentSize = MarkdownListParserOptions.DefaultListIndentSize,
            MaxListDepth = MarkdownListParserOptions.DefaultMaxListDepth,
        }, inlineParser);
        var markdownCodeBlockParser = new MarkdownCodeBlockParser();
        var markdownTableParser = new MarkdownTableParser(inlineParser);
        var markdownQuoteParser = new MarkdownQuoteParser(inlineParser);
        var markdownImageParser = new MarkdownImageParser();
        IImageSourceResolver imageSourceResolver = new ImageSourceResolver();
        IImageMetadataReader imageMetadataReader = new ImageMetadataReader(imageHttpClient, imageSourceResolver);
        IImageSizeCalculator imageSizeCalculator = new ImageSizeCalculator(options);
        var inlineRenderer = new InlineContentRenderer();
        var generatedBlockRenderer = new GeneratedBlockRenderer(
            new ParagraphBlockRenderer(inlineRenderer),
            new HeadingBlockRenderer(inlineRenderer),
            new ListBlockRenderer(inlineRenderer),
            new CodeBlockRenderer(),
            new QuoteBlockRenderer(inlineRenderer));
        return new LocalPublisherServices(
            new SimpleMarkdownParser(
                markdownCodeBlockParser,
                markdownListParser,
                markdownTableParser,
                markdownQuoteParser,
                markdownImageParser,
                inlineParser),
            new DocumentCompiler(generatedBlockRenderer),
            inlineRenderer,
            imageSourceResolver,
            imageMetadataReader,
            imageSizeCalculator);
    }

    private static PublisherCliSettings LoadSettings(bool requireGooglePublishSettings)
    {
        var settings = new PublisherCliSettings();
        ApplySettings(settings, Path.Combine(AppContext.BaseDirectory, "appsettings.json"));
        ApplySettings(settings, Path.Combine(AppContext.BaseDirectory, "appsettings.local.json"));
        ApplyEnvironment(settings);
        ValidateSettings(settings, requireGooglePublishSettings);
        return settings;
    }

    private static void ApplySettings(PublisherCliSettings settings, string settingsPath)
    {
        if (!File.Exists(settingsPath))
        {
            return;
        }

        using var document = JsonDocument.Parse(File.ReadAllText(settingsPath));
        if (document.RootElement.TryGetProperty("Google", out var legacyGoogle))
        {
            ApplyGoogleSettings(settings.Google, legacyGoogle);
        }

        if (document.RootElement.TryGetProperty("GoogleApi", out var googleApi))
        {
            ApplyGoogleSettings(settings.Google, googleApi);
        }

        if (document.RootElement.TryGetProperty("Publisher", out var publisher))
        {
            ApplyPublisherSettings(settings.Publisher, publisher);
        }

        if (document.RootElement.TryGetProperty("Cli", out var cli))
        {
            ApplyCliSettings(settings.Cli, cli);
        }
    }

    private static void ApplyEnvironment(PublisherCliSettings settings)
    {
        settings.Google.AuthenticationMode = ParseAuthenticationMode(
            Environment.GetEnvironmentVariable("VMF_PUBLISHER_AUTHENTICATION_MODE")
                ?? Environment.GetEnvironmentVariable("VMF_PUBLISHER_GOOGLE_AUTH_MODE"),
            settings.Google.AuthenticationMode);
        settings.Google.CredentialsPath = FirstEnvironment(
            "VMF_PUBLISHER_CREDENTIALS_PATH",
            "VMF_PUBLISHER_GOOGLE_CREDENTIALS_PATH") ?? settings.Google.CredentialsPath;
        settings.Google.TokenStorePath = FirstEnvironment(
            "VMF_PUBLISHER_TOKEN_STORE_PATH",
            "VMF_PUBLISHER_GOOGLE_TOKEN_STORE_PATH") ?? settings.Google.TokenStorePath;
        settings.Google.FolderId = FirstEnvironment(
            "VMF_PUBLISHER_FOLDER_ID",
            "VMF_PUBLISHER_GOOGLE_E2E_FOLDER_ID") ?? settings.Google.FolderId;
        settings.Google.TemporaryImageFolderId = Environment.GetEnvironmentVariable(
            "VMF_PUBLISHER_TEMPORARY_IMAGE_FOLDER_ID") ?? settings.Google.TemporaryImageFolderId;
        settings.Publisher.AllowTemporaryPublicImageHosting = ParseBoolean(
            Environment.GetEnvironmentVariable("VMF_PUBLISHER_ALLOW_TEMPORARY_PUBLIC_IMAGE_HOSTING"),
            settings.Publisher.AllowTemporaryPublicImageHosting);
        settings.Publisher.AllowImageUpscale = ParseBoolean(
            Environment.GetEnvironmentVariable("VMF_PUBLISHER_ALLOW_IMAGE_UPSCALE"),
            settings.Publisher.AllowImageUpscale);
        settings.Publisher.ImageMaxWidthPoints = ParseDouble(
            Environment.GetEnvironmentVariable("VMF_PUBLISHER_IMAGE_MAX_WIDTH_POINTS"),
            settings.Publisher.ImageMaxWidthPoints);
        settings.Cli.OperationTimeoutSeconds = ParsePositiveInteger(
            Environment.GetEnvironmentVariable("VMF_PUBLISHER_OPERATION_TIMEOUT_SECONDS"),
            settings.Cli.OperationTimeoutSeconds);
        settings.Cli.HttpTimeoutSeconds = ParsePositiveInteger(
            Environment.GetEnvironmentVariable("VMF_PUBLISHER_HTTP_TIMEOUT_SECONDS"),
            settings.Cli.HttpTimeoutSeconds);
    }

    private static void ApplyGoogleSettings(GooglePublisherOptions options, JsonElement settings)
    {
        options.AuthenticationMode = ParseAuthenticationMode(
            GetString(settings, "AuthenticationMode"),
            options.AuthenticationMode);
        options.CredentialsPath = GetString(settings, "CredentialsPath") ?? options.CredentialsPath;
        options.TokenStorePath = GetString(settings, "TokenStorePath") ?? options.TokenStorePath;
        options.FolderId = GetString(settings, "FolderId") ?? options.FolderId;
        options.TemporaryImageFolderId = GetString(settings, "TemporaryImageFolderId")
            ?? options.TemporaryImageFolderId;
        options.ApplicationName = GetString(settings, "ApplicationName") ?? options.ApplicationName;
    }

    private static void ApplyPublisherSettings(PublisherOptions options, JsonElement settings)
    {
        if (settings.TryGetProperty("AllowTemporaryPublicImageHosting", out var publicHosting) &&
            publicHosting.ValueKind is JsonValueKind.True or JsonValueKind.False)
        {
            options.AllowTemporaryPublicImageHosting = publicHosting.GetBoolean();
        }

        if (settings.TryGetProperty("AllowImageUpscale", out var upscale) &&
            upscale.ValueKind is JsonValueKind.True or JsonValueKind.False)
        {
            options.AllowImageUpscale = upscale.GetBoolean();
        }

        if (settings.TryGetProperty("ImageMaxWidthPoints", out var maxWidth) &&
            maxWidth.TryGetDouble(out var width))
        {
            options.ImageMaxWidthPoints = width;
        }
    }

    private static void ApplyCliSettings(CliOptions options, JsonElement settings)
    {
        options.OperationTimeoutSeconds = GetInt32(settings, "OperationTimeoutSeconds")
            ?? options.OperationTimeoutSeconds;
        options.HttpTimeoutSeconds = GetInt32(settings, "HttpTimeoutSeconds")
            ?? options.HttpTimeoutSeconds;
    }

    private static void ValidateSettings(PublisherCliSettings settings, bool requireGooglePublishSettings)
    {
        if (settings.Publisher.ImageMaxWidthPoints <= 0)
        {
            throw new CliConfigurationException(
                "CONFIG_IMAGE_MAX_WIDTH_INVALID",
                "Publisher:ImageMaxWidthPoints must be greater than zero.");
        }

        if (settings.Cli.OperationTimeoutSeconds <= 0 || settings.Cli.HttpTimeoutSeconds <= 0)
        {
            throw new CliConfigurationException(
                "CONFIG_TIMEOUT_INVALID",
                "Cli timeout values must be greater than zero.");
        }

        if (!requireGooglePublishSettings)
        {
            return;
        }

        RequireSetting(settings.Google.CredentialsPath, "CONFIG_CREDENTIALS_PATH_REQUIRED", "GoogleApi:CredentialsPath");
        RequireSetting(settings.Google.FolderId, "CONFIG_FOLDER_ID_REQUIRED", "GoogleApi:FolderId");
        if (settings.Google.AuthenticationMode == GoogleAuthenticationMode.OAuthDesktop)
        {
            RequireSetting(
                settings.Google.TokenStorePath,
                "CONFIG_TOKEN_STORE_PATH_REQUIRED",
                "GoogleApi:TokenStorePath");
        }
    }

    private static void ValidateMarkdownPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new CliConfigurationException("MARKDOWN_PATH_REQUIRED", "Markdown file path is required.");
        }

        if (!File.Exists(path))
        {
            throw new CliConfigurationException(
                "MARKDOWN_FILE_NOT_FOUND",
                $"Markdown file was not found: {path}");
        }
    }

    private static CancellationTokenSource CreateTimeoutSource(int timeoutSeconds, CancellationToken parentToken)
    {
        var source = CancellationTokenSource.CreateLinkedTokenSource(parentToken);
        source.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        return source;
    }

    private static void RequireSetting(string? value, string code, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new CliConfigurationException(code, $"{name} is required.");
        }
    }

    private static ErrorClassification Classify(string? errorCode)
    {
        if (string.IsNullOrWhiteSpace(errorCode))
        {
            return ErrorClassification.Internal;
        }

        if (errorCode.Contains("TIMEOUT", StringComparison.OrdinalIgnoreCase) ||
            errorCode.Contains("TRANSIENT", StringComparison.OrdinalIgnoreCase) ||
            errorCode is "HTTP_429" or "HTTP_500" or "HTTP_502" or "HTTP_503" or "HTTP_504")
        {
            return ErrorClassification.Transient;
        }

        if (errorCode.StartsWith("CONFIG_", StringComparison.Ordinal))
        {
            return ErrorClassification.Configuration;
        }

        if (errorCode.StartsWith("MARKDOWN_", StringComparison.Ordinal) ||
            errorCode.StartsWith("PUBLISH_INVALID_", StringComparison.Ordinal))
        {
            return ErrorClassification.Input;
        }

        return ErrorClassification.Internal;
    }

    private static bool IsTransient(Exception exception) =>
        exception is HttpRequestException
        {
            StatusCode: HttpStatusCode.TooManyRequests or
            HttpStatusCode.InternalServerError or
            HttpStatusCode.BadGateway or
            HttpStatusCode.ServiceUnavailable or
            HttpStatusCode.GatewayTimeout
        } ||
        exception.InnerException is not null && IsTransient(exception.InnerException);

    private static string SafeMessage(Exception exception) =>
        string.IsNullOrWhiteSpace(exception.Message) ? exception.GetType().Name : exception.Message;

    private static bool IsHelp(string value) => value is "-h" or "--help" or "help";

    private static string CreateSessionId() =>
        "pub-" + DateTimeOffset.UtcNow.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture) +
        "-" + Guid.NewGuid().ToString("N")[..8];

    private static string? FirstEnvironment(params string[] names) =>
        names.Select(Environment.GetEnvironmentVariable).FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));

    private static bool ParseBoolean(string? value, bool fallback) =>
        string.IsNullOrWhiteSpace(value)
            ? fallback
            : bool.TryParse(value, out var parsed)
                ? parsed
                : throw new CliConfigurationException(
                    "CONFIG_BOOLEAN_INVALID",
                    $"Invalid Boolean setting value: {value}");

    private static double ParseDouble(string? value, double fallback) =>
        string.IsNullOrWhiteSpace(value)
            ? fallback
            : double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed)
                ? parsed
                : throw new CliConfigurationException(
                    "CONFIG_NUMBER_INVALID",
                    $"Invalid numeric setting value: {value}");

    private static int ParsePositiveInteger(string? value, int fallback) =>
        string.IsNullOrWhiteSpace(value)
            ? fallback
            : int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var parsed) && parsed > 0
                ? parsed
                : throw new CliConfigurationException(
                    "CONFIG_INTEGER_INVALID",
                    $"Invalid positive integer setting value: {value}");

    private static GoogleAuthenticationMode ParseAuthenticationMode(
        string? value,
        GoogleAuthenticationMode fallback)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return fallback;
        }

        return Enum.TryParse<GoogleAuthenticationMode>(value, ignoreCase: true, out var mode)
            ? mode
            : throw new CliConfigurationException(
                "CONFIG_AUTHENTICATION_MODE_INVALID",
                $"Unsupported GoogleApi:AuthenticationMode: {value}");
    }

    private static string? GetString(JsonElement element, string propertyName) =>
        element.TryGetProperty(propertyName, out var property) ? property.GetString() : null;

    private static int? GetInt32(JsonElement element, string propertyName) =>
        element.TryGetProperty(propertyName, out var property) && property.TryGetInt32(out var value)
            ? value
            : null;
}

internal sealed class StructuredPublisherLogger : IPublisherLogger
{
    private readonly string sessionId;

    internal StructuredPublisherLogger(string sessionId)
    {
        this.sessionId = sessionId;
    }

    public void Warning(string code, string message) =>
        Write("warning", code, message, null);

    internal void Info(string code, string message) =>
        Write("info", code, message, null);

    internal void PublishPlan(string code, CompiledDocument document) =>
        Write("info", code, "Local publish plan compiled.", new Dictionary<string, object?>
        {
            ["title"] = document.Title,
            ["stepCount"] = document.Steps.Count,
        });

    internal void DiffSummary(CompiledDocument before, CompiledDocument after) =>
        Write("info", "DIFF_SUMMARY", "Local diff summary compiled.", new Dictionary<string, object?>
        {
            ["beforeTitle"] = before.Title,
            ["afterTitle"] = after.Title,
            ["beforeStepCount"] = before.Steps.Count,
            ["afterStepCount"] = after.Steps.Count,
            ["stepDelta"] = after.Steps.Count - before.Steps.Count,
        });

    internal void Summary(CliResult result, TimeSpan elapsed) =>
        Write(result.Succeeded ? "info" : "error", result.Code, result.Message, new Dictionary<string, object?>
        {
            ["exitCode"] = result.ExitCode,
            ["classification"] = result.Classification.ToString(),
            ["elapsedMilliseconds"] = (long)elapsed.TotalMilliseconds,
            ["documentId"] = result.DocumentId,
            ["documentUrl"] = result.DocumentUrl,
        });

    private void Write(string level, string code, string message, Dictionary<string, object?>? values)
    {
        var payload = new Dictionary<string, object?>
        {
            ["timestampUtc"] = DateTimeOffset.UtcNow.ToString("O", CultureInfo.InvariantCulture),
            ["level"] = level,
            ["sessionId"] = sessionId,
            ["code"] = code,
            ["message"] = message,
        };
        if (values is not null)
        {
            foreach (var pair in values)
            {
                if (pair.Value is not null)
                {
                    payload[pair.Key] = pair.Value;
                }
            }
        }

        Console.Error.WriteLine(JsonSerializer.Serialize(payload));
    }
}

internal sealed class CliConfigurationException : Exception
{
    internal CliConfigurationException(string code, string message)
        : base(message)
    {
        Code = code;
    }

    internal string Code { get; }
}

internal sealed class PublisherCliSettings
{
    internal GooglePublisherOptions Google { get; } = new();

    internal PublisherOptions Publisher { get; } = new();

    internal CliOptions Cli { get; } = new();
}

internal sealed class CliOptions
{
    internal int OperationTimeoutSeconds { get; set; } = 300;

    internal int HttpTimeoutSeconds { get; set; } = 100;
}

internal sealed record CliResult(
    int ExitCode,
    string Code,
    string Message,
    ErrorClassification Classification,
    string? DocumentId,
    string? DocumentUrl)
{
    internal bool Succeeded => ExitCode == 0;

    internal static CliResult Success(
        string code,
        string message,
        string? documentId = null,
        string? documentUrl = null) =>
        new(0, code, message, ErrorClassification.None, documentId, documentUrl);

    internal static CliResult Failure(
        int exitCode,
        string code,
        string message,
        ErrorClassification classification) =>
        new(exitCode, code, message, classification, null, null);
}

internal enum ErrorClassification
{
    None,
    Usage,
    Input,
    Configuration,
    Transient,
    Canceled,
    Internal,
}

internal sealed record LocalPublisherServices(
    IMarkdownParser Parser,
    IDocumentCompiler Compiler,
    InlineContentRenderer InlineRenderer,
    IImageSourceResolver ImageSourceResolver,
    IImageMetadataReader ImageMetadataReader,
    IImageSizeCalculator ImageSizeCalculator);
