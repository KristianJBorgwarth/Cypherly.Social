using FakeItEasy;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Social.Application.Contracts.Clients;
using Social.Application.Contracts.Repositories;
using Social.Application.Features.UserProfile.Consumers;
using Social.Domain.Aggregates;
using Social.Domain.ValueObjects;
using Social.Infrastructure.Persistence.Context;
using Social.Test.Integration.Setup;

namespace Social.Test.Integration.UserProfileTest.ConsumerTest;

public class ConnectionIdProxyConsumerTest : IntegrationTestBase
{
    private readonly ConnectionIdsProxyConsumer _sut;
    private readonly IConnectionIdProvider _fakeProvider;

    public ConnectionIdProxyConsumerTest(IntegrationTestFactory<Program, SocialDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        _fakeProvider = A.Fake<IConnectionIdProvider>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ConnectionIdsProxyConsumer>>();
        _sut = new ConnectionIdsProxyConsumer(repo, _fakeProvider, logger);
    }

    [Fact]
    public async Task Should_Call_ConnectionIdProvider_For_Single_Tenant()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "TestUser", UserTag.Create("TestUser"));

        var friendProfile = new UserProfile(Guid.NewGuid(), "FriendUser", UserTag.Create("FriendUser"));

        userProfile.AddFriendship(friendProfile);
        Db.UserProfile.Add(userProfile);
        Db.UserProfile.Add(friendProfile);
        await Db.SaveChangesAsync();

        var fe = await Db.Friendship.FirstOrDefaultAsync();
        fe!.AcceptFriendship();
        await Db.SaveChangesAsync();

        var message = new Cypherly.Message.Contracts.Messages.Device.ConnectionIdsProxyMessage
        {
            TenantId = userProfile.Id,
            CorrelationId = Guid.NewGuid(),
        };


        A.CallTo(() => _fakeProvider.GetConnectionIdsMultipleTenants(
            A<Guid[]>.That.Matches(ids => ids.Contains(userProfile.Id) && ids.Contains(friendProfile.Id)),
            A<CancellationToken>._))
            .Returns(Task.FromResult(new Dictionary<Guid, List<Guid>>
            {
                { friendProfile.Id, new List<Guid> { Guid.NewGuid(), Guid.NewGuid() } }
            }));

        var context = A.Fake<ConsumeContext<Cypherly.Message.Contracts.Messages.Device.ConnectionIdsProxyMessage>>();
        A.CallTo(() => context.Message).Returns(message);

        // Act
        await _sut.Consume(context);

        // Assert
        A.CallTo(() => _fakeProvider.GetConnectionIdsMultipleTenants(
            A<Guid[]>.That.Matches(ids => ids.Contains(userProfile.Id) && ids.Contains(friendProfile.Id)),
            A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }
}
