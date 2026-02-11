using PoLingual.Shared.Models;

namespace PoLingual.Web.Services.News;

public interface INewsService
{
    Task<List<NewsHeadline>> GetTopHeadlinesAsync(int count);
}
