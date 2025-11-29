using Social.Infrastructure.Persistence.Context;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
    
    public GetUserProfileQueryHandlerTest(IntegrationTestFactory<Program, SocialDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var profilePictureService = scope.ServiceProvider.GetRequiredService<IProfilePictureService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<GetUserProfileQueryHandler>>();

        _sut = new(repo, profilePictureService, logger);
    }

    [Fact]
    public async Task Handle_Query_With_Valid_ID_Should_Return_UserProfile()
    {
        // Arrange
        var exclusiveConnectionId = Guid.NewGuid();
        var userProfile = new UserProfile(Guid.NewGuid(), "David", UserTag.Create("David"));
        Db.UserProfile.Add(userProfile);
        await Db.SaveChangesAsync();


        var query = new GetUserProfileQuery { TenantId = userProfile.Id }; 

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_Query_With_Invalid_ID_Should_Return_NotFound()
    {
        // Arrange
        var query = new GetUserProfileQuery { TenantId = Guid.NewGuid() }; 

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Code.Should().Be("entity.not.found");
    }
}
