using PoLingual.Web.Extensions;
using PoLingual.Web.Services.News;

namespace PoLingual.Web.Endpoints;

/// <summary>
/// Minimal API endpoints for debate topics. Falls back to defaults when NewsAPI is unavailable.
/// </summary>
public static class TopicEndpoints
{
    public static IEndpointRouteBuilder MapTopicEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/topics").WithTags("Topics").WithOpenApi();

        group.MapGet("/", async (INewsService newsService, ILogger<Program> logger) =>
        {
            try
            {
                var headlines = await newsService.GetTopHeadlinesAsync(10);
                if (headlines.Count > 0)
                {
                    var topics = headlines.Select(h => new { Title = h.Title, Category = "News", Description = h.Description ?? "" }).ToList();
                    return Results.Ok(topics);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to fetch news headlines");
            }

            return Results.Ok(TopicMapperExtensions.GetDefaultTopics());
        })
        .WithName("GetTopics")
        .WithSummary("Retrieves debate topics from news headlines or defaults.");

        return endpoints;
    }
}
