using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Options;
using PoLingual.Web.Configuration;
using PoLingual.Web.Validators;

namespace PoLingual.Web.Services.AudioSynthesis;

/// <summary>
/// Azure Speech REST API implementation for audio synthesis.
/// Used by the Translation feature for Victorian English TTS.
/// </summary>
public sealed class AudioSynthesisService : IAudioSynthesisService
{
    private readonly ApiSettings _settings;
    private readonly ISpeechConfigValidator _configValidator;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AudioSynthesisService> _logger;
    private readonly TimeProvider _timeProvider;
    private string? _accessToken;
    private DateTimeOffset _tokenExpiry = DateTimeOffset.MinValue;
    private const string VoiceName = "en-GB-RyanNeural";

    public AudioSynthesisService(IOptions<ApiSettings> apiSettings, ISpeechConfigValidator configValidator,
        IHttpClientFactory httpClientFactory, ILogger<AudioSynthesisService> logger, TimeProvider timeProvider)
    {
        _settings = apiSettings.Value;
        _configValidator = configValidator;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _timeProvider = timeProvider;
    }

    private async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        if (_accessToken is not null && _timeProvider.GetUtcNow() < _tokenExpiry.AddMinutes(-1))
            return _accessToken;

        var endpoint = $"https://{_settings.AzureSpeechRegion}.api.cognitive.microsoft.com/sts/v1.0/issueToken";
        using var client = _httpClientFactory.CreateClient("SpeechToken");
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _settings.AzureSpeechSubscriptionKey);
        var response = await client.PostAsync(endpoint, new StringContent(string.Empty), cancellationToken);
        response.EnsureSuccessStatusCode();
        _accessToken = await response.Content.ReadAsStringAsync(cancellationToken);
        _tokenExpiry = _timeProvider.GetUtcNow().AddMinutes(9);
        return _accessToken;
    }

    public async Task<byte[]> SynthesizeSpeechAsync(string text, CancellationToken cancellationToken = default)
    {
        if (!_configValidator.IsValid(_settings))
            throw new InvalidOperationException($"Azure Speech not configured: {_configValidator.GetValidationError(_settings)}");

        var token = await GetAccessTokenAsync(cancellationToken);
        var ttsEndpoint = $"https://{_settings.AzureSpeechRegion}.tts.speech.microsoft.com/cognitiveservices/v1";
        var ssml = $"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='en-GB'>" +
                   $"<voice name='{VoiceName}'>{System.Security.SecurityElement.Escape(text)}</voice></speak>";

        using var client = _httpClientFactory.CreateClient("SpeechSynthesis");
        using var request = new HttpRequestMessage(HttpMethod.Post, ttsEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("X-Microsoft-OutputFormat", "audio-16khz-32kbitrate-mono-mp3");
        request.Headers.Add("User-Agent", "PoLingual");
        request.Content = new StringContent(ssml, Encoding.UTF8, "application/ssml+xml");

        var response = await client.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Speech synthesis failed ({response.StatusCode}): {error}");
        }

        return await response.Content.ReadAsByteArrayAsync(cancellationToken);
    }
}
