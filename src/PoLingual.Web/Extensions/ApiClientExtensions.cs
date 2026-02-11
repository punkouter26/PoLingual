using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace PoLingual.Web.Extensions;

/// <summary>
/// Configures HTTP clients with retry/timeout policies using Microsoft.Extensions.Http.Resilience.
/// </summary>
public static class ApiClientExtensions
{
    public static IServiceCollection AddPoLingualHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient("SpeechToken", client =>
        {
            client.Timeout = TimeSpan.FromSeconds(10);
        }).AddStandardResilienceHandler();

        services.AddHttpClient("SpeechSynthesis", client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
        }).AddStandardResilienceHandler();

        services.AddHttpClient("NewsApi", client =>
        {
            client.BaseAddress = new Uri("https://newsapi.org/");
            client.Timeout = TimeSpan.FromSeconds(15);
        }).AddStandardResilienceHandler();

        return services;
    }
}
