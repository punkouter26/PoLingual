using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using PoLingual.Shared.Models;
using PoLingual.Web.Services.AI;

namespace PoLingual.UnitTests.Services;

public class AzureOpenAIServiceTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<ILogger<AzureOpenAIService>> _loggerMock;

    public AzureOpenAIServiceTests()
    {
        _configurationMock = new Mock<IConfiguration>();
        _loggerMock = new Mock<ILogger<AzureOpenAIService>>();
    }

    [Fact]
    public void Constructor_WithMissingEndpoint_MarksServiceAsUnconfigured()
    {
        // Arrange
        _configurationMock.Setup(c => c["Azure:OpenAI:Endpoint"]).Returns("");
        _configurationMock.Setup(c => c["Azure:OpenAI:ApiKey"]).Returns("test-key");
        _configurationMock.Setup(c => c["Azure:OpenAI:DeploymentName"]).Returns("gpt-4o");

        // Act
        var service = new AzureOpenAIService(_configurationMock.Object, _loggerMock.Object);

        // Assert
        service.IsConfigured.Should().BeFalse();
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("not configured")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Constructor_WithMissingApiKey_MarksServiceAsUnconfigured()
    {
        // Arrange
        _configurationMock.Setup(c => c["Azure:OpenAI:Endpoint"]).Returns("https://test.openai.azure.com/");
        _configurationMock.Setup(c => c["Azure:OpenAI:ApiKey"]).Returns("");
        _configurationMock.Setup(c => c["Azure:OpenAI:DeploymentName"]).Returns("gpt-4o");

        // Act
        var service = new AzureOpenAIService(_configurationMock.Object, _loggerMock.Object);

        // Assert
        service.IsConfigured.Should().BeFalse();
    }

    [Fact]
    public void Constructor_WithValidConfiguration_MarksServiceAsConfigured()
    {
        // Arrange
        _configurationMock.Setup(c => c["Azure:OpenAI:Endpoint"]).Returns("https://test.openai.azure.com/");
        _configurationMock.Setup(c => c["Azure:OpenAI:ApiKey"]).Returns("test-api-key");
        _configurationMock.Setup(c => c["Azure:OpenAI:DeploymentName"]).Returns("gpt-4o");

        // Act
        var service = new AzureOpenAIService(_configurationMock.Object, _loggerMock.Object);

        // Assert
        service.IsConfigured.Should().BeTrue();
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("initialized with endpoint")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Constructor_WithMissingDeploymentName_UsesDefaultGpt4o()
    {
        // Arrange
        _configurationMock.Setup(c => c["Azure:OpenAI:Endpoint"]).Returns("https://test.openai.azure.com/");
        _configurationMock.Setup(c => c["Azure:OpenAI:ApiKey"]).Returns("test-api-key");
        _configurationMock.Setup(c => c["Azure:OpenAI:DeploymentName"]).Returns((string?)null);

        // Act
        var service = new AzureOpenAIService(_configurationMock.Object, _loggerMock.Object);

        // Assert
        service.IsConfigured.Should().BeTrue();
    }

    [Fact]
    public async Task GenerateDebateTurnAsync_WithUnconfiguredService_ThrowsInvalidOperationException()
    {
        // Arrange
        _configurationMock.Setup(c => c["Azure:OpenAI:Endpoint"]).Returns("");
        _configurationMock.Setup(c => c["Azure:OpenAI:ApiKey"]).Returns("");
        var service = new AzureOpenAIService(_configurationMock.Object, _loggerMock.Object);

        // Act
        var act = async () => await service.GenerateDebateTurnAsync("Test prompt", 100, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Azure OpenAI service is not configured.");
    }

    [Fact]
    public async Task JudgeDebateAsync_WithUnconfiguredService_ThrowsInvalidOperationException()
    {
        // Arrange
        _configurationMock.Setup(c => c["Azure:OpenAI:Endpoint"]).Returns("");
        _configurationMock.Setup(c => c["Azure:OpenAI:ApiKey"]).Returns("");
        var service = new AzureOpenAIService(_configurationMock.Object, _loggerMock.Object);

        // Act
        var act = async () => await service.JudgeDebateAsync(
            "transcript", "Rapper1", "Rapper2", "topic", CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Azure OpenAI service is not configured.");
    }

    [Fact]
    public void IsConfigured_WhenEndpointAndKeyAreSet_ReturnsTrue()
    {
        // Arrange
        _configurationMock.Setup(c => c["Azure:OpenAI:Endpoint"]).Returns("https://test.openai.azure.com/");
        _configurationMock.Setup(c => c["Azure:OpenAI:ApiKey"]).Returns("valid-key");
        _configurationMock.Setup(c => c["Azure:OpenAI:DeploymentName"]).Returns("gpt-4o");

        // Act
        var service = new AzureOpenAIService(_configurationMock.Object, _loggerMock.Object);

        // Assert
        service.IsConfigured.Should().BeTrue();
    }

    [Fact]
    public void IsConfigured_WhenEndpointIsMissing_ReturnsFalse()
    {
        // Arrange
        _configurationMock.Setup(c => c["Azure:OpenAI:Endpoint"]).Returns("");
        _configurationMock.Setup(c => c["Azure:OpenAI:ApiKey"]).Returns("valid-key");

        // Act
        var service = new AzureOpenAIService(_configurationMock.Object, _loggerMock.Object);

        // Assert
        service.IsConfigured.Should().BeFalse();
    }

    [Fact]
    public void IsConfigured_WhenApiKeyIsMissing_ReturnsFalse()
    {
        // Arrange
        _configurationMock.Setup(c => c["Azure:OpenAI:Endpoint"]).Returns("https://test.openai.azure.com/");
        _configurationMock.Setup(c => c["Azure:OpenAI:ApiKey"]).Returns("");

        // Act
        var service = new AzureOpenAIService(_configurationMock.Object, _loggerMock.Object);

        // Assert
        service.IsConfigured.Should().BeFalse();
    }

    [Fact]
    public void Constructor_LogsWarningWhenNotConfigured()
    {
        // Arrange
        _configurationMock.Setup(c => c["Azure:OpenAI:Endpoint"]).Returns((string?)null);
        _configurationMock.Setup(c => c["Azure:OpenAI:ApiKey"]).Returns((string?)null);

        // Act
        var service = new AzureOpenAIService(_configurationMock.Object, _loggerMock.Object);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("AI features will be unavailable")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Constructor_WithNullConfiguration_MarksAsUnconfigured()
    {
        // Arrange
        _configurationMock.Setup(c => c["Azure:OpenAI:Endpoint"]).Returns((string?)null);
        _configurationMock.Setup(c => c["Azure:OpenAI:ApiKey"]).Returns((string?)null);
        _configurationMock.Setup(c => c["Azure:OpenAI:DeploymentName"]).Returns((string?)null);

        // Act
        var service = new AzureOpenAIService(_configurationMock.Object, _loggerMock.Object);

        // Assert
        service.IsConfigured.Should().BeFalse();
    }
}
