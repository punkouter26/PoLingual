using System.Net;

namespace PoLingual.IntegrationTests;

[Collection("Integration")]
public class DiagnosticsEndpointTests
{
    private readonly HttpClient _client;

    public DiagnosticsEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Diagnostics_Endpoint_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/diagnostics");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DebateState_Endpoint_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/debate/state");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
