namespace PoLingual.Web.Validators;

public class InputValidator : IInputValidator
{
    private const int MaxTranslationLength = 5000;
    private const int MaxSongTitleLength = 200;

    public ValidationResult ValidateDebateInput(string? rapper1Id, string? rapper2Id, string? topicTitle)
    {
        if (string.IsNullOrWhiteSpace(rapper1Id))
            return ValidationResult.Failure("Rapper 1 must be selected.");
        if (string.IsNullOrWhiteSpace(rapper2Id))
            return ValidationResult.Failure("Rapper 2 must be selected.");
        if (string.IsNullOrWhiteSpace(topicTitle))
            return ValidationResult.Failure("Topic must be selected.");
        if (rapper1Id == rapper2Id)
            return ValidationResult.Failure("Rappers must be different.");
        return ValidationResult.Success();
    }

    public ValidationResult ValidateTranslationInput(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return ValidationResult.Failure("Translation text cannot be empty.");
        if (text.Length > MaxTranslationLength)
            return ValidationResult.Failure($"Text exceeds maximum length of {MaxTranslationLength} characters.");
        return ValidationResult.Success();
    }

    public ValidationResult ValidateLyricsInput(string? songTitle)
    {
        if (string.IsNullOrWhiteSpace(songTitle))
            return ValidationResult.Failure("Song title cannot be empty.");
        if (songTitle.Length > MaxSongTitleLength)
            return ValidationResult.Failure($"Song title exceeds maximum length of {MaxSongTitleLength} characters.");
        return ValidationResult.Success();
    }
}
