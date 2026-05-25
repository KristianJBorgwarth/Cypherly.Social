using Social.Application.Features.UserProfile.Commands.Update.TogglePrivacy;
using FluentAssertions;
using Xunit;

namespace Social.Test.Unit.UserProfileTest.CommandTest.UpdateTest.TogglePrivacy;

public class TogglePrivacyCommandValidatorTest
{
    private readonly TogglePrivacyCommandValidator _sut = new();

    [Fact]
    public void Handle_WhenIdIsEmpty_ShouldReturnValidationError()
    {
        // Arrange
        var command = new TogglePrivacyCommand
        {
            TenantId = Guid.Empty,
            IsPrivate = false
        };

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(TogglePrivacyCommand.TenantId));
    }

    [Fact]
    public void Handle_WhenIdIsNotEmpty_ShouldReturnValidationError()
    {
        // Arrange
        var command = new TogglePrivacyCommand
        {
            TenantId = Guid.NewGuid(),
            IsPrivate = false
        };

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

}
