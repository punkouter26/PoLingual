using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using PoLingual.Shared.Models;
using PoLingual.Web.Services.News;
using System.Net;
using System.Text.Json;

namespace PoLingual.UnitTests.Services;

public class NewsServiceTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<ILogger<NewsService>> _loggerMock;

    public NewsServiceTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _configurationMock = new Mock<IConfiguration>();
        _loggerMock = new Mock<ILogger<NewsService>>();
    }

    [Fact]
    public async Task GetTopHeadlinesAsync_WithValidApiKeyAndResponse_ReturnsNewsHeadlines()
    {
        // Arrange
        _configurationMock.Setup(c => c["NewsApi:ApiKey"]).Returns("test-api-key");

        var newsApiResponse = new
        {
            Status = "ok",
            Articles = new[]
            {
                new { Title = "Breaking News 1", Url = "https://example.com/1" },
                new { Title = "Breaking News 2", Url = "https://example.com/2" }
            }
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(newsApiResponse))
            });

        var service = new NewsService(_httpClient, _configurationMock.Object, _loggerMock.Object);

        // Act
        var result = await service.GetTopHeadlinesAsync(2);

        // Assert
        result.Should().HaveCount(2);
        result[0].Title.Should().Be("Breaking News 1");
        result[0].Url.Should().Be("https://example.com/1");
        result[1].Title.Should().Be("Breaking News 2");
    }

    [Fact]
    public async Task GetTopHeadlinesAsync_WithMissingApiKey_ReturnsFallbackTopics()
    {
        // Arrange
        _configurationMock.Setup(c => c["NewsApi:ApiKey"]).Returns("");
        var service = new NewsService(_httpClient, _configurationMock.Object, _loggerMock.Object);

        // Act
        var result = await service.GetTopHeadlinesAsync(3);

        // Assert
        result.Should().HaveCount(3);
        result.Should().AllSatisfy(h => h.Title.Should().NotBeNullOrWhiteSpace());
    }

    [Fact]
    public async Task GetTopHeadlinesAsync_WithApiError_ReturnsFallbackTopics()
    {
        // Arrange
        _configurationMock.Setup(c => c["NewsApi:ApiKey"]).Returns("test-api-key");

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        var service = new NewsService(_httpClient, _configurationMock.Object, _loggerMock.Object);

        // Act
        var result = await service.GetTopHeadlinesAsync(5);

        // Assert
        result.Should().HaveCount(5);
        result.Should().AllSatisfy(h => h.Title.Should().NotBeNullOrWhiteSpace());
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error fetching news")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetTopHeadlinesAsync_WithEmptyArticles_ReturnsFallbackTopics()
    {
        // Arrange
        _configurationMock.Setup(c => c["NewsApi:ApiKey"]).Returns("test-api-key");

        var newsApiResponse = new
        {
            Status = "ok",
            Articles = Array.Empty<object>()
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(newsApiResponse))
            });

        var service = new NewsService(_httpClient, _configurationMock.Object, _loggerMock.Object);

        // Act
        var result = await service.GetTopHeadlinesAsync(3);

        // Assert
        result.Should().HaveCount(3);
        result.Should().AllSatisfy(h => h.Title.Should().NotBeNullOrWhiteSpace());
    }

    [Fact]
    public async Task GetTopHeadlinesAsync_FiltersOutEmptyTitles()
    {
        // Arrange
        _configurationMock.Setup(c => c["NewsApi:ApiKey"]).Returns("test-api-key");

        var newsApiResponse = new
        {
            Status = "ok",
            Articles = new[]
            {
                new { Title = "Valid News", Url = "https://example.com/1" },
                new { Title = "", Url = "https://example.com/2" },
                new { Title = "Another Valid News", Url = "https://example.com/3" }
            }
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(newsApiResponse))
            });

        var service = new NewsService(_httpClient, _configurationMock.Object, _loggerMock.Object);

        // Act
        var result = await service.GetTopHeadlinesAsync(5);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(h => h.Title.Should().NotBeNullOrWhiteSpace());
    }

    [Fact]
    public async Task GetTopHeadlinesAsync_RespectsCountParameter()
    {
        // Arrange
        _configurationMock.Setup(c => c["NewsApi:ApiKey"]).Returns("test-api-key");

        var newsApiResponse = new
        {
            Status = "ok",
            Articles = Enumerable.Range(1, 10)
                .Select(i => new { Title = $"News {i}", Url = $"https://example.com/{i}" })
                .ToArray()
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(newsApiResponse))
            });

        var service = new NewsService(_httpClient, _configurationMock.Object, _loggerMock.Object);

        // Act
        var result = await service.GetTopHeadlinesAsync(3);

        // Assert
        result.Should().HaveCount(3);
    }
}
