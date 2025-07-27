using FluentAssertions;
using Social.Domain.Services;
using Xunit;

namespace Cypherly.UserManagement.Test.Unit.ServicesTest
{
    public class UserProfileLifecycleLifecycleServiceTest
    {
        private readonly IUserProfileLifecycleService _sut = new UserProfileLifecycleLifecycleService();

        [Fact]
        public void CreateUserProfile_ShouldReturnValidUserProfile_WhenCalledWithValidInput()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var username = "TestUser";

            // Act
            var userProfile = _sut.CreateUserProfile(userId, username);

            // Assert
            userProfile.Should().NotBeNull();
            userProfile.Id.Should().Be(userId);
            userProfile.Username.Should().Be(username);
        }

        [Fact]
        public void CreateUserProfile_ShouldGenerateCorrectUserTag()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var username = "AnotherUser";

            // Act
            var userProfile = _sut.CreateUserProfile(userId, username);

            // Assert
            userProfile.UserTag.Should().NotBeNull();
            userProfile.UserTag.Tag.Should().Contain(username);
        }
    }
}
