using Social.Application.Features.UserProfile.Queries.GetUserProfile;
using Cypherly.UserManagement.Domain.Common;
using FluentValidation.TestHelper;
using Xunit;

namespace Cypherly.UserManagement.Test.Unit.UserProfileTest.QueryTest.GetUserProfile
{
    public class GetUserProfileQueryValidatorTest
    {
        private readonly GetUserProfileQueryValidator _validator;

        public GetUserProfileQueryValidatorTest()
        {
            _validator = new GetUserProfileQueryValidator();
        }

        [Fact]
        public void Should_Have_Error_When_UserProfileId_Is_Null()
        {
            // Arrange
            var query = new Social.Application.Features.UserProfile.Queries.GetUserProfile.GetUserProfileQuery { UserProfileId = Guid.Empty, ExlusiveConnectionId = Guid.NewGuid() };

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UserProfileId)
                .WithErrorMessage(Errors.General.ValueIsEmpty(nameof(Social.Application.Features.UserProfile.Queries.GetUserProfile.GetUserProfileQuery.UserProfileId)).Message);
        }

        [Fact]
        public void Should_Have_Error_When_UserProfileId_Is_Empty()
        {
            // Arrange
            var query = new Social.Application.Features.UserProfile.Queries.GetUserProfile.GetUserProfileQuery { UserProfileId = Guid.Empty, ExlusiveConnectionId = Guid.Empty}; // Empty Guid

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UserProfileId)
                .WithErrorMessage(Errors.General.ValueIsEmpty(nameof(Social.Application.Features.UserProfile.Queries.GetUserProfile.GetUserProfileQuery.UserProfileId)).Message);
        }

        [Fact]
        public void Should_Not_Have_Error_When_UserProfileId_Is_Valid()
        {
            // Arrange
            var query = new Social.Application.Features.UserProfile.Queries.GetUserProfile.GetUserProfileQuery { UserProfileId = Guid.NewGuid(), ExlusiveConnectionId = Guid.NewGuid()};

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.UserProfileId);
        }
    }
}