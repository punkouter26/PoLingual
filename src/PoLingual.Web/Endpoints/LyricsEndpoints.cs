using PoLingual.Web.Services.Lyrics;

namespace PoLingual.Web.Endpoints;

/// <summary>
/// Minimal API endpoints for song lyrics retrieval.
/// </summary>
public static class LyricsEndpoints
{
    public static IEndpointRouteBuilder MapLyricsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/lyrics").WithTags("Lyrics").WithOpenApi();

        group.MapGet("/songs", async (ILyricsService lyricsService) =>
        {
            var songs = await lyricsService.GetAvailableSongsAsync();
            return Results.Ok(songs);
        })
        .WithName("GetAvailableSongs")
        .WithSummary("Retrieves list of available song titles.");

        group.MapGet("/{songTitle}", async (string songTitle, ILyricsService lyricsService) =>
        {
            var lyrics = await lyricsService.GetLyricsAsync(songTitle);
            return lyrics is not null ? Results.Ok(new { Title = songTitle, Lyrics = lyrics }) : Results.NotFound();
        })
        .WithName("GetLyrics")
        .WithSummary("Retrieves lyrics for a specific song.");

        group.MapGet("/random", async (ILyricsService lyricsService) =>
        {
            var (title, lyrics) = await lyricsService.GetRandomLyricsAsync();
            return title is not null ? Results.Ok(new { Title = title, Lyrics = lyrics }) : Results.NotFound();
        })
        .WithName("GetRandomLyrics")
        .WithSummary("Retrieves random song lyrics for translation.");

        return endpoints;
    }
}
