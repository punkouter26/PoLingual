namespace PoLingual.Web.Services.Lyrics;

public interface ILyricsService
{
    Task<List<string>> GetAvailableSongsAsync(CancellationToken cancellationToken = default);
    Task<string?> GetLyricsAsync(string songTitle, CancellationToken cancellationToken = default);
    Task<(string? Title, string? Lyrics)> GetRandomLyricsAsync(CancellationToken cancellationToken = default);
}
