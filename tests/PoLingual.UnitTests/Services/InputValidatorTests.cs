using PoLingual.Web.Validators;

namespace PoLingual.UnitTests.Services;

public class InputValidatorTests
{
    private readonly InputValidator _sut = new();

    [Fact]
    public void ValidateDebateInput_ValidInput_ReturnsSuccess()
    {
        var result = _sut.ValidateDebateInput("rapper1", "rapper2", "topic");
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null, "r2", "topic", "Rapper 1 must be selected.")]
    [InlineData("r1", null, "topic", "Rapper 2 must be selected.")]
    [InlineData("r1", "r2", null, "Topic must be selected.")]
    [InlineData("same", "same", "topic", "Rappers must be different.")]
    public void ValidateDebateInput_InvalidInput_ReturnsFailure(string? r1, string? r2, string? topic, string expectedError)
    {
        var result = _sut.ValidateDebateInput(r1, r2, topic);
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Be(expectedError);
    }

    [Fact]
    public void ValidateTranslationInput_ValidText_ReturnsSuccess()
    {
        var result = _sut.ValidateTranslationInput("Hello world");
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateTranslationInput_EmptyText_ReturnsFailure()
    {
        var result = _sut.ValidateTranslationInput("");
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateTranslationInput_TooLongText_ReturnsFailure()
    {
        var result = _sut.ValidateTranslationInput(new string('x', 5001));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateLyricsInput_ValidTitle_ReturnsSuccess()
    {
        var result = _sut.ValidateLyricsInput("My Song");
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateLyricsInput_EmptyTitle_ReturnsFailure()
    {
        var result = _sut.ValidateLyricsInput("");
        result.IsValid.Should().BeFalse();
    }
}
