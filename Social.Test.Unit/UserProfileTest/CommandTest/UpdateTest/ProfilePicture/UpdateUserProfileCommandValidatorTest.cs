using Social.Application.Features.UserProfile.Commands.Update.ProfilePicture;
using FakeItEasy;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http;
using Social.Domain.Common;
using Xunit;

namespace Social.Test.Unit.UserProfileTest.CommandTest.UpdateTest.ProfilePicture;

public class UpdateChatUserProfilePictureCommandValidatorTests
{
    private readonly UpdateUserProfilePictureCommandValidator _validator = new();

    [Fact]
    public void Validate_GivenValidCommand_ShouldBeValid()
    {
        // Arrange
        var command = new UpdateUserProfilePictureCommand
        {
            TenantId = Guid.NewGuid(),
            NewProfilePicture = A.Fake<IFormFile>()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_GivenEmptyId_ShouldBeInvalid()
    {
        // Arrange
        var command = new UpdateUserProfilePictureCommand()
        {
            TenantId = Guid.Empty,
            NewProfilePicture = A.Fake<IFormFile>()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.TenantId)
            .WithErrorMessage(Errors.General.ValueIsEmpty(nameof(UpdateUserProfilePictureCommand.TenantId)).Message);
    }

    [Fact]
    public void Validate_GivenNullNewProfilePicture_ShouldBeInvalid()
    {
        // Arrange
        var command = new UpdateUserProfilePictureCommand
        {
            TenantId = Guid.NewGuid(),
            NewProfilePicture = null
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.NewProfilePicture)
            .WithErrorMessage(Errors.General.ValueIsRequired(nameof(UpdateUserProfilePictureCommand.NewProfilePicture)).Message);
    }

    [Fact]
    public void Validate_GivenEmptyNewProfilePicture_ShouldBeInvalid()
    {
        // Arrange
        var command = new UpdateUserProfilePictureCommand
        {
            TenantId = Guid.NewGuid(),
            NewProfilePicture = null
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.NewProfilePicture)
            .WithErrorMessage(Errors.General.ValueIsEmpty(nameof(UpdateUserProfilePictureCommand.NewProfilePicture)).Message);
    }
}
