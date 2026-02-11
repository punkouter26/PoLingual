using System.Net;
using System.Net.Http.Json;
using PoLingual.Shared.Models;

namespace PoLingual.IntegrationTests;

[Collection("Integration")]
public class RapperEndpointTests
{
    private readonly HttpClient _client;

    public RapperEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetRappers_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/rappers");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetTopics_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/topics");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
