using Azure;
using Azure.AI.OpenAI;
using PoLingual.Shared.Models;
using OpenAI.Chat;
using System.Text.Json;

namespace PoLingual.Web.Services.AI;

/// <summary>
/// Azure OpenAI service for generating rap debate content and judging debates.
/// Uses Strategy pattern for AI interactions with retry logic.
/// </summary>
public class AzureOpenAIService : IAzureOpenAIService
{
    private static readonly JsonSerializerOptions s_jsonOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly AzureOpenAIClient? _openAIClient;
    private readonly ChatClient? _chatClient;
    private readonly ILogger<AzureOpenAIService> _logger;
    private readonly string _openAIDeploymentName;

    public bool IsConfigured { get; }

    public AzureOpenAIService(IConfiguration configuration, ILogger<AzureOpenAIService> logger)
    {
        _logger = logger;
        var openAIEndpoint = configuration["Azure:OpenAI:Endpoint"];
        var openAIApiKey = configuration["Azure:OpenAI:ApiKey"];
        _openAIDeploymentName = configuration["Azure:OpenAI:DeploymentName"] ?? "gpt-4o";

        if (string.IsNullOrEmpty(openAIEndpoint) || string.IsNullOrEmpty(openAIApiKey))
        {
            _logger.LogWarning("Azure OpenAI endpoint or API key is not configured. AI features will be unavailable.");
            IsConfigured = false;
            return;
        }

        _openAIClient = new AzureOpenAIClient(new Uri(openAIEndpoint), new AzureKeyCredential(openAIApiKey));
        _chatClient = _openAIClient.GetChatClient(_openAIDeploymentName);
        IsConfigured = true;
        _logger.LogInformation("Azure OpenAI client initialized with endpoint: {Endpoint}", openAIEndpoint);
    }

    public async Task<string> GenerateDebateTurnAsync(string prompt, int maxTokens, CancellationToken cancellationToken)
    {
        if (!IsConfigured || _chatClient is null)
            throw new InvalidOperationException("Azure OpenAI service is not configured.");

        _logger.LogInformation("Generating debate turn with max tokens: {MaxTokens}", maxTokens);
        const int maxRetries = 3;
        Exception? lastException = null;

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                var messages = new List<ChatMessage>
                {
                    new SystemChatMessage("You are a rap battle AI. Generate creative, clean, and impactful rap lyrics. Keep responses concise."),
                    new UserChatMessage(prompt)
                };

                var options = new ChatCompletionOptions() { Temperature = 0.7f, MaxOutputTokenCount = maxTokens };
                var response = await _chatClient.CompleteChatAsync(messages, options, cancellationToken);

                if (response.Value.Content.Count == 0)
                {
                    lastException = new InvalidOperationException("OpenAI returned empty content");
                    await Task.Delay(1000 * attempt, cancellationToken);
                    continue;
                }

                _logger.LogInformation("Generated debate turn successfully on attempt {Attempt}", attempt);
                return response.Value.Content[0].Text;
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error generating debate turn on attempt {Attempt}/{MaxRetries}", attempt, maxRetries);
                lastException = ex;
                if (attempt < maxRetries) await Task.Delay(1000 * attempt, cancellationToken);
            }
        }

        throw lastException ?? new InvalidOperationException("Failed to generate debate turn after retries");
    }

    public async Task<JudgeDebateResponse> JudgeDebateAsync(string debateTranscript, string rapper1Name, string rapper2Name, string topic, CancellationToken cancellationToken)
    {
        if (!IsConfigured || _chatClient is null)
            throw new InvalidOperationException("Azure OpenAI service is not configured.");

        _logger.LogInformation("Judging debate for topic: {Topic}", topic);
        try
        {
            var systemPrompt = @"You are an impartial rap battle judge. Analyze the debate transcript between " +
                               $"{rapper1Name} and {rapper2Name} on '{topic}'. " +
                               "Determine a winner based on lyrical skill, relevance, creativity, and impact. " +
                               "Respond in JSON: {\"WinnerName\":\"...\",\"Reasoning\":\"...\",\"Stats\":{\"Rapper1Score\":N,\"Rapper2Score\":N}}";

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(systemPrompt),
                new UserChatMessage($"Debate Transcript:\n{debateTranscript}")
            };

            var options = new ChatCompletionOptions() { Temperature = 0.5f, ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat() };
            var response = await _chatClient.CompleteChatAsync(messages, options, cancellationToken);
            string jsonResponse = response.Value.Content[0].Text;

            var judgeResponse = JsonSerializer.Deserialize<JudgeDebateResponse>(jsonResponse, s_jsonOptions);
            if (judgeResponse == null)
                return new JudgeDebateResponse { WinnerName = "Error Parsing", Reasoning = "Failed to parse judge's response.", Stats = new DebateStats() };

            judgeResponse.Stats ??= new DebateStats();
            return judgeResponse;
        }
        catch (OperationCanceledException) { throw; }
        catch (JsonException jsonEx)
        {
            _logger.LogError(jsonEx, "JSON deserialization error during debate judging.");
            return new JudgeDebateResponse { WinnerName = "Error Parsing", Reasoning = $"JSON parsing error: {jsonEx.Message}", Stats = new DebateStats() };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error judging debate from OpenAI.");
            throw;
        }
    }
}
