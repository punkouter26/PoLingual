using PoLingual.Web.Services.News;

namespace PoLingual.Web.Endpoints;

/// <summary>
/// Minimal API endpoints for news headlines.
/// </summary>
public static class NewsEndpoints
{
    public static IEndpointRouteBuilder MapNewsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/news").WithTags("News").WithOpenApi();

        group.MapGet("/headlines", async (INewsService newsService) =>
        {
            var headlines = await newsService.GetTopHeadlinesAsync(10);
            return Results.Ok(headlines);
        })
        .WithName("GetHeadlines")
        .WithSummary("Retrieves top news headlines for debate topic inspiration.");

        return endpoints;
    }
}
