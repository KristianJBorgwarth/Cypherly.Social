using Social.Application.Contracts.Repositories;
using Social.Application.Contracts.Services;
using Social.Application.Features.UserProfile.Queries.GetUserProfileByTag;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Social.Domain.Aggregates;
using Social.Domain.Interfaces;
using Social.Domain.Services;
using Xunit;

namespace Social.Test.Unit.UserProfileTest.QueryTest.GetUserProfileByTag;

public class GetUserProfileByTagQueryHandlerTest
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUserBlockingService _userBlockingService;
    private readonly IProfilePictureService _profilePictureService;
    private readonly IFriendshipService _friendshipService;

    private readonly GetUserProfileByTagQueryHandler _sut;

    public GetUserProfileByTagQueryHandlerTest()
    {
        _userProfileRepository = A.Fake<IUserProfileRepository>();
        _userBlockingService = A.Fake<IUserBlockingService>();
        _profilePictureService = A.Fake<IProfilePictureService>();
        _friendshipService = A.Fake<IFriendshipService>();
        var logger = A.Fake<ILogger<GetUserProfileByTagQueryHandler>>();

        _sut = new(_userProfileRepository, _userBlockingService, _profilePictureService, _friendshipService, logger);
    }

    [Fact]
    public async Task Handle_When_RequestingUser_Does_Not_Exist_Returns_Result_Fail()
    {
        // Arrange
        var request = new GetUserProfileByTagQuery
        {
            TenantId = Guid.NewGuid(),
            Tag = "TestTag"
        };

        A.CallTo(() => _userProfileRepository.GetByIdAsync(request.TenantId))!.Returns<UserProfile>(null);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_When_UserProfile_Is_Blocked_Returns_Result_Empty()
    {
        // Arrange
        var request = new GetUserProfileByTagQuery
        {
            TenantId = Guid.NewGuid(),
            Tag = "TestTag"
        };

        var requestingUser = new UserProfile();
        var userProfile = new UserProfile();

        A.CallTo(() => _userProfileRepository.GetByIdAsync(request.TenantId)).Returns(requestingUser);
        A.CallTo(() => _userProfileRepository.GetByUserTag(request.Tag)).Returns(userProfile);
        A.CallTo(() => _userBlockingService.IsUserBloccked(requestingUser, userProfile))!.Returns(true);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task Handle_When_UserProfile_Does_Not_Exist_Return_Empty_Result()
    {
        // Arrange
        var request = new GetUserProfileByTagQuery
        {
            TenantId = Guid.NewGuid(),
            Tag = "TestTag"
        };

        var requestingUser = new UserProfile();

        A.CallTo(() => _userProfileRepository.GetByIdAsync(request.TenantId)).Returns(requestingUser);
        A.CallTo(() => _userProfileRepository.GetByUserTag(request.Tag))!.Returns<UserProfile>(null);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().BeNull();
    }
}
