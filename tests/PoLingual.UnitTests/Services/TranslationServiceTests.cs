using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PoLingual.Web.Configuration;
using PoLingual.Web.Services.Translation;

namespace PoLingual.UnitTests.Services;

public class TranslationServiceTests
{
    private readonly Mock<ITranslationCache> _cacheMock;
    private readonly Mock<ILogger<TranslationService>> _loggerMock;
    private readonly Mock<IOptions<ApiSettings>> _optionsMock;

    public TranslationServiceTests()
    {
        _cacheMock = new Mock<ITranslationCache>();
        _loggerMock = new Mock<ILogger<TranslationService>>();
        _optionsMock = new Mock<IOptions<ApiSettings>>();
    }

    [Fact]
    public async Task Constructor_WithMissingConfiguration_MarksServiceAsUnconfigured()
    {
        // Arrange
        var settings = new ApiSettings
        {
            AzureOpenAIEndpoint = "",
            AzureOpenAIDeploymentName = "gpt-4o",
            AzureOpenAIApiKey = ""
        };
        _optionsMock.Setup(x => x.Value).Returns(settings);

        // Act
        var service = new TranslationService(_optionsMock.Object, _cacheMock.Object, _loggerMock.Object);
        var act = async () => await service.TranslateToVictorianEnglishAsync("Hello");

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Translation service is not configured.");

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
    public async Task TranslateToVictorianEnglishAsync_WithNullInput_ThrowsArgumentException()
    {
        // Arrange
        var settings = new ApiSettings
        {
            AzureOpenAIEndpoint = "https://test.openai.azure.com/",
            AzureOpenAIDeploymentName = "gpt-4o",
            AzureOpenAIApiKey = "test-key"
        };
        _optionsMock.Setup(x => x.Value).Returns(settings);
        var service = new TranslationService(_optionsMock.Object, _cacheMock.Object, _loggerMock.Object);

        // Act
        var act = async () => await service.TranslateToVictorianEnglishAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task TranslateToVictorianEnglishAsync_WithEmptyInput_ThrowsArgumentException()
    {
        // Arrange
        var settings = new ApiSettings
        {
            AzureOpenAIEndpoint = "https://test.openai.azure.com/",
            AzureOpenAIDeploymentName = "gpt-4o",
            AzureOpenAIApiKey = "test-key"
        };
        _optionsMock.Setup(x => x.Value).Returns(settings);
        var service = new TranslationService(_optionsMock.Object, _cacheMock.Object, _loggerMock.Object);

        // Act
        var act = async () => await service.TranslateToVictorianEnglishAsync("");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task TranslateToVictorianEnglishAsync_WithCachedTranslation_ReturnsCachedResult()
    {
        // Arrange
        var settings = new ApiSettings
        {
            AzureOpenAIEndpoint = "https://test.openai.azure.com/",
            AzureOpenAIDeploymentName = "gpt-4o",
            AzureOpenAIApiKey = "test-key"
        };
        _optionsMock.Setup(x => x.Value).Returns(settings);

        var cachedTranslation = "Good morrow, dear fellow!";
        _cacheMock.Setup(c => c.TryGetTranslation("Hello friend", out cachedTranslation))
            .Returns(true);

        var service = new TranslationService(_optionsMock.Object, _cacheMock.Object, _loggerMock.Object);

        // Act
        var result = await service.TranslateToVictorianEnglishAsync("Hello friend");

        // Assert
        result.Should().Be(cachedTranslation);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("cached translation")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Constructor_WithValidConfiguration_LogsInitialization()
    {
        // Arrange
        var settings = new ApiSettings
        {
            AzureOpenAIEndpoint = "https://test.openai.azure.com/",
            AzureOpenAIDeploymentName = "gpt-4o",
            AzureOpenAIApiKey = "test-key-123"
        };
        _optionsMock.Setup(x => x.Value).Returns(settings);

        // Act
        var service = new TranslationService(_optionsMock.Object, _cacheMock.Object, _loggerMock.Object);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("TranslationService initialized")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task TranslateToVictorianEnglishAsync_WithUnconfiguredService_ThrowsInvalidOperationException()
    {
        // Arrange
        var settings = new ApiSettings
        {
            AzureOpenAIEndpoint = "",
            AzureOpenAIDeploymentName = "",
            AzureOpenAIApiKey = ""
        };
        _optionsMock.Setup(x => x.Value).Returns(settings);
        var service = new TranslationService(_optionsMock.Object, _cacheMock.Object, _loggerMock.Object);

        // Act
        var act = async () => await service.TranslateToVictorianEnglishAsync("Test");

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Translation service is not configured.");
    }

    [Theory]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public async Task TranslateToVictorianEnglishAsync_WithWhitespaceInput_ThrowsArgumentException(string input)
    {
        // Arrange
        var settings = new ApiSettings
        {
            AzureOpenAIEndpoint = "https://test.openai.azure.com/",
            AzureOpenAIDeploymentName = "gpt-4o",
            AzureOpenAIApiKey = "test-key"
        };
        _optionsMock.Setup(x => x.Value).Returns(settings);
        var service = new TranslationService(_optionsMock.Object, _cacheMock.Object, _loggerMock.Object);

        // Act
        var act = async () => await service.TranslateToVictorianEnglishAsync(input);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }
}
