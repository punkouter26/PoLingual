using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using PoLingual.Web.Configuration;
using System.ClientModel;
using System.Diagnostics;

namespace PoLingual.Web.Services.Translation;

/// <summary>
/// Translates modern English to Victorian-era English using Azure OpenAI.
/// Includes caching for cost optimization.
/// </summary>
public sealed class TranslationService : ITranslationService
{
    private readonly ChatClient? _chatClient;
    private readonly ITranslationCache _cache;
    private readonly ILogger<TranslationService> _logger;
    private readonly bool _isConfigured;
    private const int MaxTokens = 500;

    private const string SystemPrompt = """
        Translate modern English to Victorian-era style. Use period vocabulary and formal prose.
        Output only the translated text, no explanations.
        """;

    public TranslationService(IOptions<ApiSettings> apiSettings, ITranslationCache cache, ILogger<TranslationService> logger)
    {
        _cache = cache;
        _logger = logger;
        var settings = apiSettings.Value;

        if (string.IsNullOrWhiteSpace(settings.AzureOpenAIEndpoint) ||
            string.IsNullOrWhiteSpace(settings.AzureOpenAIDeploymentName) ||
            string.IsNullOrWhiteSpace(settings.AzureOpenAIApiKey))
        {
            _logger.LogWarning("Azure OpenAI settings not configured for Translation. Feature will be unavailable.");
            _isConfigured = false;
            return;
        }

        var openAIClient = new AzureOpenAIClient(new Uri(settings.AzureOpenAIEndpoint), new ApiKeyCredential(settings.AzureOpenAIApiKey));
        _chatClient = openAIClient.GetChatClient(settings.AzureOpenAIDeploymentName);
        _isConfigured = true;
        _logger.LogInformation("TranslationService initialized with endpoint {Endpoint}", settings.AzureOpenAIEndpoint);
    }

    public async Task<string> TranslateToVictorianEnglishAsync(string modernText, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(modernText);

        if (!_isConfigured || _chatClient is null)
            throw new InvalidOperationException("Translation service is not configured.");

        if (_cache.TryGetTranslation(modernText, out var cached) && cached is not null)
        {
            _logger.LogInformation("Returning cached translation");
            return cached;
        }

        var stopwatch = Stopwatch.StartNew();
        var messages = new ChatMessage[]
        {
            new SystemChatMessage(SystemPrompt),
            new UserChatMessage(modernText)
        };
        var options = new ChatCompletionOptions { MaxOutputTokenCount = MaxTokens, Temperature = 0.7f };

        var response = await _chatClient.CompleteChatAsync(messages, options, cancellationToken);
        var translatedText = response.Value.Content[0].Text;

        stopwatch.Stop();
        _logger.LogInformation("Translation completed in {Duration}ms. Tokens: {In}+{Out}",
            stopwatch.ElapsedMilliseconds, response.Value.Usage.InputTokenCount, response.Value.Usage.OutputTokenCount);

        _cache.CacheTranslation(modernText, translatedText);
        return translatedText;
    }
}
