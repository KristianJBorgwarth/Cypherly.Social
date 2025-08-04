using Social.Application.Features.UserProfile.Commands.Create.Friendship;
using FluentValidation.TestHelper;
using Social.Domain.Common;
using Xunit;

namespace Social.Test.Unit.UserProfileTest.CommandTest.CreateTest.Friendship
{
    public class CreateFriendshipCommandValidatorTest
    {
        private readonly CreateFriendshipCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_UserId_Is_Empty()
        {
            // Arrange
            var command = new CreateFriendshipCommand
            {
                TenantId = Guid.Empty, // Invalid UserId
                FriendTag = "ValidTag"
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TenantId)
                .WithErrorMessage(Errors.General.ValueIsEmpty(nameof(CreateFriendshipCommand.TenantId)).Message);
        }

        [Fact]
        public void Should_Have_Error_When_FriendTag_Is_Null()
        {
            // Arrange
            var command = new CreateFriendshipCommand
            {
                TenantId = Guid.NewGuid(),
                FriendTag = null // Null FriendTag
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.FriendTag)
                .WithErrorMessage(Errors.General.ValueIsRequired(nameof(CreateFriendshipCommand.FriendTag)).Message);
        }

        [Fact]
        public void Should_Have_Error_When_FriendTag_Is_Empty()
        {
            // Arrange
            var command = new CreateFriendshipCommand
            {
                TenantId = Guid.NewGuid(),
                FriendTag = "" // Empty FriendTag
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.FriendTag)
                .WithErrorMessage(Errors.General.ValueIsEmpty(nameof(CreateFriendshipCommand.FriendTag)).Message);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Command_Is_Valid()
        {
            // Arrange
            var command = new CreateFriendshipCommand
            {
                TenantId = Guid.NewGuid(), // Valid UserId
                FriendTag = "ValidTag"   // Valid FriendTag
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.TenantId);
            result.ShouldNotHaveValidationErrorFor(x => x.FriendTag);
        }
    }
}
