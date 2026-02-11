namespace PoLingual.Shared.Models;

/// <summary>
/// Request model for translation.
/// </summary>
public sealed record TranslationRequest(string Text);

/// <summary>
/// Response model for translation results.
/// </summary>
public sealed record TranslationResponse(
    string OriginalText,
    string TranslatedText,
    byte[]? AudioData);
