using AutoFixture;
using Cypherly.Message.Contracts.Enums;
using Cypherly.Message.Contracts.Messages.User;
using Social.Infrastructure.Persistence.Context;
using FakeItEasy;
using FluentAssertions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Social.Application.Contracts.Repositories;
using Social.Application.Features.UserProfile.Consumers;
using Social.Domain.Aggregates;
using Social.Domain.Services;
using Social.Domain.ValueObjects;
using Social.Test.Integration.Setup;

namespace Social.Test.Integration.UserProfileTest.ConsumerTest;

public class RollbackUserProfileDeleteConsumerTest : IntegrationTestBase
{
    private readonly RollbackUserProfileDeleteConsumer _sut;

    public RollbackUserProfileDeleteConsumerTest(IntegrationTestFactory<Program, SocialDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var userProfileRepository = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var userProfileService = scope.ServiceProvider.GetRequiredService<IUserProfileLifecycleService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<RollbackUserProfileDeleteConsumer>>();
        _sut = new(userProfileRepository, userProfileService, unitOfWork, logger);
    }

    [Fact]
    public async Task Consume_Valid_Message_Should_RevertSoftDelete_UserProfile()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "James", UserTag.Create("james"));
        userProfile.SetDelete();
        await Db.UserProfile.AddAsync(userProfile);
        await Db.SaveChangesAsync();

        var message = Fixture.Build<UserDeleteFailedMessage>()
            .With(x => x.UserId, userProfile.Id)
            .With(x => x.Services, [ServiceType.UserManagementService])
            .Create();

        var fakeConsumeContext = A.Fake<ConsumeContext<UserDeleteFailedMessage>>();
        A.CallTo(() => fakeConsumeContext.Message).Returns(message);

        // Act
        await _sut.Consume(fakeConsumeContext);

        // Assert
        Db.UserProfile.AsNoTracking().FirstOrDefault()!.Deleted.Should().BeNull();
    }

    [Fact]
    public async Task Consume_Invalid_Message_Should_Not_RevertSoftDelete_UserProfile()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "James", UserTag.Create("james"));
        userProfile.SetDelete();
        await Db.UserProfile.AddAsync(userProfile);
        await Db.SaveChangesAsync();

        //Invalid ID
        var message = Fixture.Create<UserDeleteFailedMessage>();

        var fakeConsumeContext = A.Fake<ConsumeContext<UserDeleteFailedMessage>>();
        A.CallTo(() => fakeConsumeContext.Message).Returns(message);

        // Act
        await _sut.Consume(fakeConsumeContext);

        // Assert
        Db.UserProfile.AsNoTracking().FirstOrDefault()!.Deleted.Should().NotBeNull();
    }
}