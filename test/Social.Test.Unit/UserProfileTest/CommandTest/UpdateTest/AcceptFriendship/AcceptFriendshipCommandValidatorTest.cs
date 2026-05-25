using FluentAssertions;
using Social.Application.Features.Friendships.Commands.Update.AcceptFriendship;
using Xunit;

namespace Social.Test.Unit.UserProfileTest.CommandTest.UpdateTest.AcceptFriendship
{
    public class AcceptFriendshipCommandValidatorTest
    {
        private readonly AcceptFriendshipCommandValidator _sut = new AcceptFriendshipCommandValidator();

        [Fact]
        public void Validator_ShouldReturnSuccess_WhenCommandIsValid()
        {
            // Arrange
            var command = new AcceptFriendshipCommand
            {
                TenantId = Guid.NewGuid(),
                FriendTag = "ValidFriendTag"
            };

            // Act
            var result = _sut.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validator_ShouldReturnFailure_WhenUserIdIsEmpty()
        {
            // Arrange
            var command = new AcceptFriendshipCommand
            {
                TenantId = Guid.Empty, // Invalid UserId
                FriendTag = "ValidFriendTag"
            };

            // Act
            var result = _sut.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(AcceptFriendshipCommand.TenantId) && e.ErrorMessage == $"The value cannot be empty: {nameof(AcceptFriendshipCommand.TenantId)} ");
        }

        [Fact]
        public void Validator_ShouldReturnFailure_WhenFriendTagIsNull()
        {
            // Arrange
            var command = new AcceptFriendshipCommand
            {
                TenantId = Guid.NewGuid(),
                FriendTag = null // Invalid FriendTag
            };

            // Act
            var result = _sut.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(AcceptFriendshipCommand.FriendTag) && e.ErrorMessage == $"Value '{nameof(AcceptFriendshipCommand.FriendTag)}' is required.");
        }

        [Fact]
        public void Validator_ShouldReturnFailure_WhenFriendTagIsEmpty()
        {
            // Arrange
            var command = new AcceptFriendshipCommand
            {
                TenantId = Guid.NewGuid(),
                FriendTag = "" // Invalid FriendTag
            };

            // Act
            var result = _sut.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == nameof(AcceptFriendshipCommand.FriendTag) && e.ErrorMessage == $"The value cannot be empty: {nameof(AcceptFriendshipCommand.FriendTag)} ");
        }
    }
}
