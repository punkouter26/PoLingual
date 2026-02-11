namespace PoLingual.Shared.Models;

/// <summary>
/// Represents a news headline fetched from an external API.
/// </summary>
public class NewsHeadline
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Url { get; set; }
    public string? SourceName { get; set; }
}

public class NewsApiResponse
{
    public string? Status { get; set; }
    public int TotalResults { get; set; }
    public List<NewsApiArticle>? Articles { get; set; }
}

public class NewsApiArticle
{
    public NewsApiSource? Source { get; set; }
    public string? Author { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Url { get; set; }
    public string? UrlToImage { get; set; }
    public DateTime PublishedAt { get; set; }
    public string? Content { get; set; }
}

public class NewsApiSource
{
    public string? Id { get; set; }
    public string? Name { get; set; }
}
