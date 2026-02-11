using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using PoLingual.Web.Services.Translation;

namespace PoLingual.UnitTests.Services;

public class TranslationCacheTests
{
    private readonly TranslationCache _sut;

    public TranslationCacheTests()
    {
        var cache = new MemoryCache(new MemoryCacheOptions { SizeLimit = 100 });
        _sut = new TranslationCache(cache, NullLogger<TranslationCache>.Instance);
    }

    [Fact]
    public void TryGetTranslation_EmptyCache_ReturnsFalse()
    {
        var result = _sut.TryGetTranslation("hello", out var cached);
        result.Should().BeFalse();
        cached.Should().BeNull();
    }

    [Fact]
    public void CacheAndRetrieve_ReturnsTrue()
    {
        _sut.CacheTranslation("hello", "Good day!");
        var result = _sut.TryGetTranslation("hello", out var cached);
        result.Should().BeTrue();
        cached.Should().Be("Good day!");
    }

    [Fact]
    public void DifferentKeys_ReturnIndependentResults()
    {
        _sut.CacheTranslation("hello", "Good day!");
        _sut.CacheTranslation("goodbye", "Farewell!");

        _sut.TryGetTranslation("hello", out var v1);
        _sut.TryGetTranslation("goodbye", out var v2);

        v1.Should().Be("Good day!");
        v2.Should().Be("Farewell!");
    }
}
