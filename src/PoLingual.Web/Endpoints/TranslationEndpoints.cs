using FluentValidation;
using PoLingual.Shared.Models;
using PoLingual.Web.Services.Translation;
using PoLingual.Web.Services.Telemetry;
using System.Diagnostics;

namespace PoLingual.Web.Endpoints;

/// <summary>
/// Minimal API endpoints for Victorian English translation.
/// </summary>
public static class TranslationEndpoints
{
    public static IEndpointRouteBuilder MapTranslationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/translation").WithTags("Translation").WithOpenApi();

        group.MapPost("/", async (TranslationRequest request, ITranslationService translationService,
            IValidator<TranslationRequest> validator, DebateMetrics metrics) =>
        {
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
                return Results.BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

            var stopwatch = Stopwatch.StartNew();
            var translated = await translationService.TranslateToVictorianEnglishAsync(request.Text);
            stopwatch.Stop();

            metrics.RecordTranslationRequested(stopwatch.ElapsedMilliseconds);

            return Results.Ok(new TranslationResponse(request.Text, translated, null));
        })
        .WithName("TranslateText")
        .WithSummary("Translates modern English to Victorian-era English.")
        .Produces<TranslationResponse>()
        .ProducesValidationProblem();

        return endpoints;
    }
}
