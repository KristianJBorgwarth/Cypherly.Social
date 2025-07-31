using Social.Infrastructure.Persistence.Context;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Social.Application.Contracts.Clients;
using Social.Application.Contracts.Repositories;
using Social.Application.Contracts.Services;
using Social.Application.Features.UserProfile.Queries.GetUserProfile;
using Social.Domain.Aggregates;
using Social.Domain.ValueObjects;
using Social.Test.Integration.Setup;

namespace Social.Test.Integration.UserProfileTest.QueryTest.GetUserProfileTest;

public class GetUserProfileQueryHandlerTest : IntegrationTestBase
{
    private readonly GetUserProfileQueryHandler _sut;
    private readonly IConnectionIdProvider _connectionIdProvider;

    public GetUserProfileQueryHandlerTest(IntegrationTestFactory<Program, SocialDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var profilePictureService = scope.ServiceProvider.GetRequiredService<IProfilePictureService>();
        _connectionIdProvider = A.Fake<IConnectionIdProvider>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<GetUserProfileQueryHandler>>();

        _sut = new(repo, profilePictureService, _connectionIdProvider, logger);
    }

    [Fact]
    public async Task Handle_Query_With_Valid_ID_Should_Return_UserProfile()
    {
        // Arrange
        var exclusiveConnectionId = Guid.NewGuid();
        var userProfile = new UserProfile(Guid.NewGuid(), "David", UserTag.Create("David"));
        Db.UserProfile.Add(userProfile);
        await Db.SaveChangesAsync();

        A.CallTo(()=> _connectionIdProvider.GetConnectionIdsByUser(userProfile.Id, A<CancellationToken>._))
            .Returns([Guid.NewGuid(), exclusiveConnectionId]);

        var query = new GetUserProfileQuery { UserProfileId = userProfile.Id, ExclusiveConnectionId = exclusiveConnectionId};

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.ConnectionIds.Should().HaveCount(1);
        result.Value.ConnectionIds.Should().NotContain(exclusiveConnectionId);
    }

    [Fact]
    public async Task Handle_Query_With_Invalid_ID_Should_Return_NotFound()
    {
        // Arrange
        var query = new GetUserProfileQuery { UserProfileId = Guid.NewGuid(), ExclusiveConnectionId = Guid.NewGuid()};

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Code.Should().Be("entity.not.found");
    }
}