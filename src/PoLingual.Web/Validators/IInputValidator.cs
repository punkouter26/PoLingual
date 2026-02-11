namespace PoLingual.Web.Validators;

public interface IInputValidator
{
    ValidationResult ValidateDebateInput(string? rapper1Id, string? rapper2Id, string? topicTitle);
    ValidationResult ValidateTranslationInput(string? text);
    ValidationResult ValidateLyricsInput(string? songTitle);
}

public record ValidationResult(bool IsValid, string? ErrorMessage = null)
{
    public static ValidationResult Success() => new(true);
    public static ValidationResult Failure(string error) => new(false, error);
}
