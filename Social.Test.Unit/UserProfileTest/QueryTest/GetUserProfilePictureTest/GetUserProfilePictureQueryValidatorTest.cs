using FluentValidation.TestHelper;
using Xunit;
using Social.Application.Features.UserProfile.Queries.GetAvatar;

namespace Social.Test.Unit.UserProfileTest.QueryTest.GetUserProfilePictureTest;

public class GetUserProfilePictureQueryValidatorTest
{
    private readonly GetAvatarQueryValidator _validator = new GetAvatarQueryValidator();

    [Fact]
    public void Should_Have_Error_When_ProfilePictureUrl_Is_Empty()
    {
        // Arrange
        var query = new GetAvatarQuery { AvatarId = string.Empty };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AvatarId);
    }

    [Fact]
    public void Should_Not_Have_Error_When_ProfilePictureUrl_Is_Valid()
    {
        // Arrange
        var query = new GetAvatarQuery { AvatarId = "https://example.com/profile.jpg" };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.AvatarId);
    }
}
