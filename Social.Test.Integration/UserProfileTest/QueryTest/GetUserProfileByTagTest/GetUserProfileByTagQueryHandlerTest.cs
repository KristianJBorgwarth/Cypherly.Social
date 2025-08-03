using Social.Infrastructure.Persistence.Context;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Social.Application.Contracts.Repositories;
using Social.Application.Contracts.Services;
using Social.Application.Features.UserProfile.Queries.GetUserProfileByTag;
using Social.Domain.Aggregates;
using Social.Domain.Interfaces;
using Social.Domain.Services;
using Social.Domain.ValueObjects;
using Social.Test.Integration.Setup;

namespace Social.Test.Integration.UserProfileTest.QueryTest.GetUserProfileByTagTest;

public class GetUserProfileByTagQueryHandlerTest : IntegrationTestBase
{
    private readonly GetUserProfileByTagQueryHandler _sut;
    public GetUserProfileByTagQueryHandlerTest(IntegrationTestFactory<Program, SocialDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var friendshipService = scope.ServiceProvider.GetRequiredService<IFriendshipService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<GetUserProfileByTagQueryHandler>>();
        var profilePictureService = scope.ServiceProvider.GetRequiredService<IProfilePictureService>();
        var userProfileService = scope.ServiceProvider.GetRequiredService<IUserBlockingService>();

        _sut = new(repo, userProfileService, profilePictureService, friendshipService, logger);
    }

    [Fact]
    public async Task Handle_Valid_Query_Should_Return_UserProfile()
    {
        // Arrange
        var requestingUser = new UserProfile(Guid.NewGuid(), "requestingUser", UserTag.Create("requestingUser"));
        var userProfile = new UserProfile(Guid.NewGuid(), "userProfile", UserTag.Create("userProfile"));
        await Db.AddRangeAsync(requestingUser, userProfile);
        await Db.SaveChangesAsync();

        var query = new GetUserProfileByTagQuery() { TenantId = requestingUser.Id, Tag = userProfile.UserTag.Tag };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value!.Username.Should().Be(userProfile.Username);
        result.Value.UserTag.Should().Be(userProfile.UserTag.Tag);
    }

    [Fact]
    public async Task Handle_Query_When_User_Does_Not_Exist_Should_Return_NotFound()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "userProfile", UserTag.Create("userProfile"));
        await Db.AddAsync(userProfile);
        await Db.SaveChangesAsync();

        var query = new GetUserProfileByTagQuery() { TenantId = Guid.NewGuid(), Tag = userProfile.UserTag.Tag };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);


        // Assert
        result.Success.Should().BeFalse();
        result.Error!.Code.Should().Contain("entity.not.found");
    }

    [Fact]
    public async Task Handle_Query_When_UserProfile_Does_Not_Exist_Should_Return_EmptyDto()
    {
        // Arrange
        var requestingUser = new UserProfile(Guid.NewGuid(), "requestingUser", UserTag.Create("requestingUser"));
        await Db.AddAsync(requestingUser);
        await Db.SaveChangesAsync();

        var query = new GetUserProfileByTagQuery() { TenantId = requestingUser.Id, Tag = "userProfile" };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().Be(null);
    }

    [Fact]
    public async Task Handle_Query_When_User_Is_Blocked_Should_Return_EmptyDto()
    {
        var requestingUser = new UserProfile(Guid.NewGuid(), "requestingUser", UserTag.Create("requestingUser"));
        var userProfile = new UserProfile(Guid.NewGuid(), "userProfile", UserTag.Create("userProfile"));
        requestingUser.BlockUser(userProfile.Id);
        await Db.AddRangeAsync(requestingUser, userProfile);
        await Db.SaveChangesAsync();

        var query = new GetUserProfileByTagQuery() { TenantId = requestingUser.Id, Tag = userProfile.UserTag.Tag };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().Be(null);
    }
    [Fact]

    public async Task Handle_Query_When_User_Is_Private_Should_Return_EmptyDto()
    {
        var requestingUser = new UserProfile(Guid.NewGuid(), "requestingUser", UserTag.Create("requestingUser"));
        var userProfile = new UserProfile(Guid.NewGuid(), "userProfile", UserTag.Create("userProfile"));
        userProfile.TogglePrivacy(true);
        await Db.AddRangeAsync(requestingUser, userProfile);
        await Db.SaveChangesAsync();

        var query = new GetUserProfileByTagQuery() { TenantId = requestingUser.Id, Tag = userProfile.UserTag.Tag };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().Be(null);
    }
}