using Social.Application.Features.UserProfile.Commands.Update.DisplayName;
using FluentAssertions;
using Xunit;

namespace Social.Test.Unit.UserProfileTest.CommandTest.UpdateTest.DisplayName
{
    public class UpdateUserProfileDisplayNameCommandValidatorTest
    {
        private readonly UpdateUserProfileDisplayNameCommandValidator _sut = new();

        [Fact]
        public void Validate_ShouldHaveError_WhenUserProfileIdIsNull()
        {
            // Arrange
            var command = new UpdateUserProfileDisplayNameCommand
            {
                TenantId = Guid.Empty,
                DisplayName = "ValidDisplayName"
            };

            // Act
            var result = _sut.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should()
                .Be($"The value cannot be empty: {nameof(UpdateUserProfileDisplayNameCommand.TenantId)} ");
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenDisplayNameIsNull()
        {
            // Arrange
            var command = new UpdateUserProfileDisplayNameCommand
            {
                TenantId = Guid.NewGuid(),
                DisplayName = null
            };

            // Act
            var result = _sut.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should()
                .Be($"Value '{nameof(UpdateUserProfileDisplayNameCommand.DisplayName)}' is required.");
        }

        [Fact]
        public void Validate_ShouldHaveNoErrors_WhenCommandIsValid()
        {
            // Arrange
            var command = new UpdateUserProfileDisplayNameCommand
            {
                TenantId = Guid.NewGuid(),
                DisplayName = "ValidDisplayName"
            };

            // Act
            var result = _sut.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenUserProfileIdIsEmpty()
        {
            // Arrange
            var command = new UpdateUserProfileDisplayNameCommand
            {
                TenantId = Guid.Empty,
                DisplayName = "ValidDisplayName"
            };

            // Act
            var result = _sut.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should()
                .Be($"The value cannot be empty: {nameof(UpdateUserProfileDisplayNameCommand.TenantId)} ");
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenDisplayNameIsEmpty()
        {
            // Arrange
            var command = new UpdateUserProfileDisplayNameCommand
            {
                TenantId = Guid.NewGuid(),
                DisplayName = ""
            };

            // Act
            var result = _sut.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should()
                .Be($"The value cannot be empty: {nameof(UpdateUserProfileDisplayNameCommand.DisplayName)} ");
        }
    }
}
