using AutoFixture;
using Cypherly.Message.Contracts.Messages.Profile;
using Social.Infrastructure.Persistence.Context;
using FakeItEasy;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Social.Application.Contracts.Repositories;
using Social.Application.Features.UserProfile.Consumers;
using Social.Domain.Services;
using Social.Test.Integration.Setup;

namespace Social.Test.Integration.UserProfileTest.ConsumerTest;

public class CreateUserProfileConsumerTest : IntegrationTestBase
{
    private readonly CreateUserProfileConsumer _sut;
    public CreateUserProfileConsumerTest(IntegrationTestFactory<Program, UserManagementDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var userProfileRepository = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var userProfileService = scope.ServiceProvider.GetRequiredService<IUserProfileLifecycleService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<CreateUserProfileConsumer>>();
        _sut = new(userProfileRepository, unitOfWork, userProfileService, logger);
    }

    [Fact]
    public async Task Consume_ValidRequest_Should_Create_UserProfile()
    {
        // Arrange
        var request = Fixture.Build<CreateUserProfileMessage>()
            .With(x => x.Username, "testUser")
            .Create();


        var fakeConsumeContext = A.Fake<ConsumeContext<CreateUserProfileMessage>>();
        A.CallTo(() => fakeConsumeContext.Message).Returns(request);

        // Act
        await _sut.Consume(fakeConsumeContext);

        // Assert
        Db.UserProfile.Count().Should().Be(1);
        Db.UserProfile.First().Id.Should().Be(request.UserId);
        Db.UserProfile.First().Username.Should().Be(request.Username);
    }
}