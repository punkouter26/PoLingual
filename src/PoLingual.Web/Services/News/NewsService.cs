using PoLingual.Shared.Models;

namespace PoLingual.Web.Services.News;

/// <summary>
/// Fetches news headlines from NewsAPI or provides fallback topics.
/// </summary>
public class NewsService : INewsService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NewsService> _logger;
    private readonly string _newsApiKey;

    public NewsService(HttpClient httpClient, IConfiguration configuration, ILogger<NewsService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _newsApiKey = configuration["NewsApi:ApiKey"] ?? "";
        _httpClient.BaseAddress = new Uri("https://newsapi.org/v2/");
    }

    public async Task<List<NewsHeadline>> GetTopHeadlinesAsync(int count)
    {
        if (string.IsNullOrWhiteSpace(_newsApiKey))
            return GetFallbackTopics(count);

        try
        {
            var response = await _httpClient.GetFromJsonAsync<NewsApiInternalResponse>($"top-headlines?country=us&apiKey={_newsApiKey}");
            if (response?.Articles == null || response.Articles.Count == 0)
                return GetFallbackTopics(count);

            return response.Articles
                .Where(a => !string.IsNullOrWhiteSpace(a.Title))
                .Take(count)
                .Select(a => new NewsHeadline { Title = a.Title, Url = a.Url })
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error fetching news. Using fallback topics.");
            return GetFallbackTopics(count);
        }
    }

    private static List<NewsHeadline> GetFallbackTopics(int count)
    {
        var fallback = new List<NewsHeadline>
        {
            new() { Title = "Artificial Intelligence vs Human Creativity" },
            new() { Title = "Social Media Impact on Society" },
            new() { Title = "Climate Change Solutions" },
            new() { Title = "Future of Remote Work" },
            new() { Title = "Electric Cars vs Gas Cars" },
            new() { Title = "Space Exploration Priorities" },
            new() { Title = "Healthy Lifestyle Choices" },
            new() { Title = "Education System Reform" }
        };
        return fallback.OrderBy(_ => Random.Shared.Next()).Take(count).ToList();
    }

    private class NewsApiInternalResponse
    {
        public required string Status { get; set; }
        public required List<NewsApiInternalArticle> Articles { get; set; }
    }

    private class NewsApiInternalArticle
    {
        public required string Title { get; set; }
        public string? Url { get; set; }
    }
}
