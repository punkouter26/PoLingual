using Microsoft.Extensions.Caching.Memory;

namespace PoLingual.Web.Services.Translation;

/// <summary>
/// In-memory cache for translation results to reduce OpenAI API costs.
/// </summary>
public sealed class TranslationCache : ITranslationCache
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<TranslationCache> _logger;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromHours(24);
    private int _cacheCount;

    public TranslationCache(IMemoryCache memoryCache, ILogger<TranslationCache> logger)
    {
        _cache = memoryCache;
        _logger = logger;
    }

    public bool TryGetTranslation(string modernText, out string? victorianText)
    {
        var key = $"translation:{modernText.GetHashCode():X8}:{modernText.Length}";
        if (_cache.TryGetValue(key, out victorianText))
        {
            _logger.LogInformation("Cache hit (length: {Length})", modernText.Length);
            return true;
        }
        victorianText = null;
        return false;
    }

    public void CacheTranslation(string modernText, string victorianText)
    {
        if (_cacheCount >= 1000) return;
        var key = $"translation:{modernText.GetHashCode():X8}:{modernText.Length}";
        var options = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(_cacheDuration)
            .SetSize(1)
            .RegisterPostEvictionCallback((_, _, _, _) => Interlocked.Decrement(ref _cacheCount));
        _cache.Set(key, victorianText, options);
        Interlocked.Increment(ref _cacheCount);
    }
}
