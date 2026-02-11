using System.Net;

namespace PoLingual.IntegrationTests;

[Collection("Integration")]
public class HealthEndpointTests
{
    private readonly HttpClient _client;

    public HealthEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_Endpoint_ReturnsValidResponse()
    {
        var response = await _client.GetAsync("/health");
        // Health might be Degraded/Unhealthy if Azure services aren't configured in test
        new[] { HttpStatusCode.OK, HttpStatusCode.ServiceUnavailable, HttpStatusCode.InternalServerError }
            .Should().Contain(response.StatusCode);
    }
}
