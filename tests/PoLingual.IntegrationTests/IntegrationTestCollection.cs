namespace PoLingual.IntegrationTests;

/// <summary>
/// Shared test collection so all integration tests use a single WebApplicationFactory instance.
/// This prevents race conditions from multiple factories spinning up simultaneously.
/// </summary>
[CollectionDefinition("Integration")]
public class IntegrationTestCollection : ICollectionFixture<CustomWebApplicationFactory>
{
}
