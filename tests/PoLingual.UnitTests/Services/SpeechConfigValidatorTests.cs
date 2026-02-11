using PoLingual.Web.Configuration;
using PoLingual.Web.Validators;

namespace PoLingual.UnitTests.Services;

public class SpeechConfigValidatorTests
{
    private readonly SpeechConfigValidator _sut = new();

    [Fact]
    public void IsValid_WithAllSettings_ReturnsTrue()
    {
        var settings = new ApiSettings
        {
            AzureSpeechSubscriptionKey = "key123",
            AzureSpeechRegion = "eastus"
        };
        _sut.IsValid(settings).Should().BeTrue();
    }

    [Theory]
    [InlineData(null, "eastus")]
    [InlineData("key", null)]
    [InlineData("", "eastus")]
    [InlineData("key", "")]
    public void IsValid_WithMissingSettings_ReturnsFalse(string? key, string? region)
    {
        var settings = new ApiSettings
        {
            AzureSpeechSubscriptionKey = key!,
            AzureSpeechRegion = region!
        };
        _sut.IsValid(settings).Should().BeFalse();
    }

    [Fact]
    public void GetValidationError_MissingKey_ReturnsKeyError()
    {
        var settings = new ApiSettings { AzureSpeechRegion = "eastus" };
        var error = _sut.GetValidationError(settings);
        error.Should().Contain("SubscriptionKey");
    }
}
