using System.Text.Json;
using System.Globalization;
using Vmf.Publisher.Application;
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
    private const string HelpText =
        """
        VMF Studio v2.0 Publisher PoC v0.1

        Usage:
          vmf-publisher publish <markdown-file>
          vmf-publisher --help

        Commands:
          publish <markdown-file>  Publish a Markdown file to Google Docs.

        Options:
          -h, --help               Show this help.
        """;

    internal static async Task<int> RunAsync(string[] arguments, CancellationToken cancellationToken)
    {
        if (arguments.Length == 0 || IsHelp(arguments[0]))
        {
            Console.WriteLine(HelpText);
            return 0;
        }

        if (!string.Equals(arguments[0], "publish", StringComparison.OrdinalIgnoreCase))
        {
            Console.Error.WriteLine($"Unknown command: {arguments[0]}");
            Console.Error.WriteLine(HelpText);
            return 2;
        }

        if (arguments.Length == 2 && IsHelp(arguments[1]))
        {
            Console.WriteLine(HelpText);
            return 0;
        }

        if (arguments.Length != 2)
        {
            Console.Error.WriteLine("publish requires exactly one Markdown file path.");
            Console.Error.WriteLine(HelpText);
            return 2;
        }

        var options = LoadOptions();
        var publisherOptions = LoadPublisherOptions();
        using var httpClient = new HttpClient();
        using var imageHttpClient = new HttpClient(new HttpClientHandler
        {
            AllowAutoRedirect = false,
        });
        var credentialProvider = GoogleCredentialProviderFactory.Create(options, httpClient);
        var requestMapper = new GoogleDocsRequestMapper();
        var serviceFactory = new GoogleServiceFactory(credentialProvider, requestMapper, httpClient);
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
        IImageMetadataReader imageMetadataReader = new ImageMetadataReader(
            imageHttpClient,
            imageSourceResolver);
        IImageSizeCalculator imageSizeCalculator = new ImageSizeCalculator(publisherOptions);
        IPublisherLogger logger = new ConsolePublisherLogger();
        ITemporaryImageHost temporaryImageHost = new GoogleDriveTemporaryImageHost(
            credentialProvider,
            httpClient,
            options,
            publisherOptions,
            logger);
        var inlineRenderer = new InlineContentRenderer();
        var paragraphBlockRenderer = new ParagraphBlockRenderer(inlineRenderer);
        var headingBlockRenderer = new HeadingBlockRenderer(inlineRenderer);
        var listBlockRenderer = new ListBlockRenderer(inlineRenderer);
        var codeBlockRenderer = new CodeBlockRenderer();
        var quoteBlockRenderer = new QuoteBlockRenderer(inlineRenderer);
        IGeneratedBlockRenderer generatedBlockRenderer = new GeneratedBlockRenderer(
            paragraphBlockRenderer,
            headingBlockRenderer,
            listBlockRenderer,
            codeBlockRenderer,
            quoteBlockRenderer);
        IPublishPlanExecutor publishPlanExecutor = new PublishPlanExecutor(
            serviceFactory.CreateDocsClient(),
            inlineRenderer,
            temporaryImageHost,
            logger);
        var googlePublisher = new GoogleDocsPublisher(serviceFactory, options, publishPlanExecutor);
        IPublishService publishService = new PublishService(
            new MarkdownFileDocumentLoader(),
            new SimpleMarkdownParser(
                markdownCodeBlockParser,
                markdownListParser,
                markdownTableParser,
                markdownQuoteParser,
                markdownImageParser,
                inlineParser),
            new DocumentCompiler(generatedBlockRenderer),
            googlePublisher,
            imageSourceResolver,
            imageMetadataReader,
            imageSizeCalculator);

        try
        {
            var result = await publishService.PublishAsync(
                new PublishRequest(arguments[1]),
                cancellationToken).ConfigureAwait(false);
            if (!result.IsSuccess)
            {
                Console.Error.WriteLine($"{result.Error?.Code}: {result.Error?.Message}");
                return 1;
            }

            Console.WriteLine("Google Drive API: success");
            Console.WriteLine("Google Docs API: success");
            Console.WriteLine($"Document ID: {result.DocumentId}");
            Console.WriteLine($"Document URL: {result.DocumentUrl}");
            return 0;
        }
        catch (OperationCanceledException)
        {
            Console.Error.WriteLine("Publication was canceled.");
            return 130;
        }
    }

    private static bool IsHelp(string value) => value is "-h" or "--help" or "help";

    private static GooglePublisherOptions LoadOptions()
    {
        var options = new GooglePublisherOptions();
        ApplySettings(options, Path.Combine(AppContext.BaseDirectory, "appsettings.json"));
        ApplySettings(options, Path.Combine(AppContext.BaseDirectory, "appsettings.local.json"));

        options.AuthenticationMode = ParseAuthenticationMode(
            Environment.GetEnvironmentVariable("VMF_PUBLISHER_AUTHENTICATION_MODE"),
            options.AuthenticationMode);
        options.CredentialsPath = Environment.GetEnvironmentVariable("VMF_PUBLISHER_CREDENTIALS_PATH")
            ?? options.CredentialsPath;
        options.TokenStorePath = Environment.GetEnvironmentVariable("VMF_PUBLISHER_TOKEN_STORE_PATH")
            ?? options.TokenStorePath;
        options.FolderId = Environment.GetEnvironmentVariable("VMF_PUBLISHER_FOLDER_ID")
            ?? options.FolderId;
        options.TemporaryImageFolderId = Environment.GetEnvironmentVariable(
            "VMF_PUBLISHER_TEMPORARY_IMAGE_FOLDER_ID") ?? options.TemporaryImageFolderId;
        return options;
    }

    private static void ApplySettings(GooglePublisherOptions options, string settingsPath)
    {
        if (File.Exists(settingsPath))
        {
            using var document = JsonDocument.Parse(File.ReadAllText(settingsPath));
            if (document.RootElement.TryGetProperty("Google", out var legacyGoogle))
            {
                ApplyGoogleSettings(options, legacyGoogle);
            }

            if (document.RootElement.TryGetProperty("GoogleApi", out var googleApi))
            {
                ApplyGoogleSettings(options, googleApi);
            }
        }
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

    private static PublisherOptions LoadPublisherOptions()
    {
        var options = new PublisherOptions();
        ApplyPublisherSettings(options, Path.Combine(AppContext.BaseDirectory, "appsettings.json"));
        ApplyPublisherSettings(options, Path.Combine(AppContext.BaseDirectory, "appsettings.local.json"));
        options.AllowTemporaryPublicImageHosting = ParseBoolean(
            Environment.GetEnvironmentVariable("VMF_PUBLISHER_ALLOW_TEMPORARY_PUBLIC_IMAGE_HOSTING"),
            options.AllowTemporaryPublicImageHosting);
        options.AllowImageUpscale = ParseBoolean(
            Environment.GetEnvironmentVariable("VMF_PUBLISHER_ALLOW_IMAGE_UPSCALE"),
            options.AllowImageUpscale);
        options.ImageMaxWidthPoints = ParseDouble(
            Environment.GetEnvironmentVariable("VMF_PUBLISHER_IMAGE_MAX_WIDTH_POINTS"),
            options.ImageMaxWidthPoints);
        return options;
    }

    private static void ApplyPublisherSettings(PublisherOptions options, string settingsPath)
    {
        if (!File.Exists(settingsPath))
        {
            return;
        }

        using var document = JsonDocument.Parse(File.ReadAllText(settingsPath));
        if (!document.RootElement.TryGetProperty("Publisher", out var settings))
        {
            return;
        }

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

    private static bool ParseBoolean(string? value, bool fallback) =>
        string.IsNullOrWhiteSpace(value)
            ? fallback
            : bool.TryParse(value, out var parsed)
                ? parsed
                : throw new InvalidOperationException($"Invalid Boolean setting value: {value}");

    private static double ParseDouble(string? value, double fallback) =>
        string.IsNullOrWhiteSpace(value)
            ? fallback
            : double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed)
                ? parsed
                : throw new InvalidOperationException($"Invalid numeric setting value: {value}");

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
            : throw new InvalidOperationException($"Unsupported GoogleApi:AuthenticationMode: {value}");
    }

    private static string? GetString(JsonElement element, string propertyName) =>
        element.TryGetProperty(propertyName, out var property) ? property.GetString() : null;
}

internal sealed class ConsolePublisherLogger : IPublisherLogger
{
    public void Warning(string code, string message) =>
        Console.Error.WriteLine($"{code}: {message}");
}
