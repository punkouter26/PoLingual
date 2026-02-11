using System.Text.Json;
using PoLingual.Shared.Models;

namespace PoLingual.Web.Services.Lyrics;

/// <summary>
/// Service for retrieving song lyrics from a JSON collection.
/// Uses lazy loading with thread-safe caching.
/// </summary>
public sealed class LyricsService : ILyricsService
{
    private readonly string _lyricsFilePath;
    private readonly ILogger<LyricsService> _logger;
    private const int MaxWords = 200;
    private LyricsCollection? _lyricsCache;
    private readonly SemaphoreSlim _loadSemaphore = new(1, 1);
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public LyricsService(IWebHostEnvironment env, ILogger<LyricsService> logger)
    {
        _logger = logger;
        _lyricsFilePath = Path.Combine(env.ContentRootPath, "Data", "lyrics-collection.json");
    }

    private async Task<LyricsCollection> GetCollectionAsync(CancellationToken cancellationToken)
    {
        if (_lyricsCache is not null) return _lyricsCache;
        await _loadSemaphore.WaitAsync(cancellationToken);
        try
        {
            if (_lyricsCache is not null) return _lyricsCache;
            if (!File.Exists(_lyricsFilePath))
                throw new FileNotFoundException($"Lyrics file not found: {_lyricsFilePath}");
            var json = await File.ReadAllTextAsync(_lyricsFilePath, cancellationToken);
            _lyricsCache = JsonSerializer.Deserialize<LyricsCollection>(json, JsonOptions)
                ?? throw new InvalidOperationException("Failed to deserialize lyrics collection");
            _logger.LogInformation("Loaded {Count} songs", _lyricsCache.Songs.Count);
            return _lyricsCache;
        }
        finally { _loadSemaphore.Release(); }
    }

    public async Task<List<string>> GetAvailableSongsAsync(CancellationToken cancellationToken = default)
    {
        var collection = await GetCollectionAsync(cancellationToken);
        return collection.Songs.Select(s => s.Title).OrderBy(t => t).ToList();
    }

    public async Task<string?> GetLyricsAsync(string songTitle, CancellationToken cancellationToken = default)
    {
        var collection = await GetCollectionAsync(cancellationToken);
        var song = collection.Songs.FirstOrDefault(s => s.Title.Equals(songTitle, StringComparison.OrdinalIgnoreCase));
        if (song is null) return null;
        var words = song.Lyrics.Split([' ', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries);
        return words.Length <= MaxWords ? song.Lyrics : string.Join(" ", words.Take(MaxWords)) + "...";
    }

    public async Task<(string? Title, string? Lyrics)> GetRandomLyricsAsync(CancellationToken cancellationToken = default)
    {
        var collection = await GetCollectionAsync(cancellationToken);
        if (collection.Songs.Count == 0) return (null, null);
        var song = collection.Songs[Random.Shared.Next(collection.Songs.Count)];
        var lyrics = await GetLyricsAsync(song.Title, cancellationToken);
        return (song.Title, lyrics);
    }
}
