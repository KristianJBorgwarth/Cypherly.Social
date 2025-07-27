using AutoFixture;
using Cypherly.Message.Contracts.Abstractions;
using Cypherly.Message.Contracts.Messages.Common;
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

public class DeleteUserProfileConsumerTest : IntegrationTestBase
{
    private readonly DeleteUserProfileConsumer _sut;
    public DeleteUserProfileConsumerTest(IntegrationTestFactory<Program, UserManagementDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var userProfileRepository = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var userProfileService = scope.ServiceProvider.GetRequiredService<IUserProfileLifecycleService>();
        var producer = scope.ServiceProvider.GetRequiredService<IProducer<OperationSucceededMessage>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DeleteUserProfileConsumer>>();
        _sut = new DeleteUserProfileConsumer(userProfileRepository, userProfileService, unitOfWork, producer, logger);
    }

    [Fact]
    public async Task Consume_Valid_Message_Should_SoftDelete_UserProfile()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "James", UserTag.Create("james"));
        await Db.UserProfile.AddAsync(userProfile);
        await Db.SaveChangesAsync();

        var message = Fixture.Build<UserDeleteMessage>()
            .With(x => x.UserProfileId, userProfile.Id).Create();

        var fakeConsumeContext = A.Fake<ConsumeContext<UserDeleteMessage>>();
        A.CallTo(() => fakeConsumeContext.Message).Returns(message);

        // Act
        await _sut.Consume(fakeConsumeContext);

        // Assert
        Db.UserProfile.AsNoTracking().FirstOrDefault()!.Deleted.Should().NotBeNull();
    }

    [Fact]
    public async Task Consume_Invalid_Message_Should_Throw_KeyNotFoundException()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "James", UserTag.Create("james"));
        await Db.UserProfile.AddAsync(userProfile);
        await Db.SaveChangesAsync();
        var invalidId = Guid.NewGuid();

        var message = Fixture.Build<UserDeleteMessage>()
            .With(x => x.UserProfileId, invalidId).Create();

        var fakeConsumeContext = A.Fake<ConsumeContext<UserDeleteMessage>>();
        A.CallTo(() => fakeConsumeContext.Message).Returns(message);

        // Act
        var act = async () => await _sut.Consume(fakeConsumeContext);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
        Db.UserProfile.AsNoTracking().FirstOrDefault()!.Deleted.Should().BeNull();
    }
}