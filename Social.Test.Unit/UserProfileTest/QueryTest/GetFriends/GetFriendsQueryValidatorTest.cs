using Social.Application.Features.UserProfile.Queries.GetFriends;
using FluentValidation.TestHelper;
using Social.Domain.Common;
using Xunit;

namespace Social.Test.Unit.UserProfileTest.QueryTest.GetFriends
{
    public class GetFriendsQueryValidatorTest
    {
        private readonly GetFriendsQueryValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_UserProfileId_Is_Null()
        {
            // Arrange
            var query = new GetFriendsQuery { TenantId = Guid.Empty };

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TenantId)
                .WithErrorMessage(Errors.General.ValueIsEmpty(nameof(GetFriendsQuery.TenantId)).Message);
        }

        [Fact]
        public void Should_Have_Error_When_UserProfileId_Is_Empty()
        {
            // Arrange
            var query = new GetFriendsQuery { TenantId = Guid.Empty }; // Empty Guid

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.TenantId)
                .WithErrorMessage(Errors.General.ValueIsEmpty(nameof(GetFriendsQuery.TenantId)).Message);
        }

        [Fact]
        public void Should_Not_Have_Error_When_UserProfileId_Is_Valid()
        {
            // Arrange
            var query = new GetFriendsQuery { TenantId = Guid.NewGuid() };

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.TenantId);
        }
    }
}