namespace PoLingual.Web.Services.Translation;

public interface ITranslationService
{
    Task<string> TranslateToVictorianEnglishAsync(string modernText, CancellationToken cancellationToken = default);
}
