using Social.Application.Features.UserProfile.Queries.GetBlockedUserProfiles;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace Social.Test.Unit.UserProfileTest.QueryTest.GetBlockedUserProfiles;

public class GetBlockedUserProfilesQueryValidatorTest
{
    private readonly GetBlockedUserProfilesQueryValidator _validator = new();

    [Fact]
    public void Validate_Given_Valid_Query_Should_Return_Ok()
    {
        // Arrange
        var query = new GetBlockedUserProfilesQuery() { TenantId = Guid.NewGuid() };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_Given_Empty_Id_Query_Should_Fail_With_Errors()
    {
        // Arrange
        var query = new GetBlockedUserProfilesQuery() { TenantId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.TenantId);
    }
}