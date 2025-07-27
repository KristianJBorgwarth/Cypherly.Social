using AutoFixture;
using Cypherly.Message.Contracts.Abstractions;
using Cypherly.Message.Contracts.Messages.Common;
using Cypherly.Message.Contracts.Messages.User;
using Social.Application.Contracts.Repositories;
using Social.Application.Features.UserProfile.Consumers;
using FakeItEasy;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Social.Domain.Aggregates;
using Social.Domain.Services;
using Xunit;

namespace Cypherly.UserManagement.Test.Unit.UserProfileTest.ConsumerTest;

public class DeleteUserProfileConsumerTest
{
    private readonly IUserProfileRepository _fakeRepo;
    private readonly IUnitOfWork _fakeUow;
    private readonly IProducer<OperationSucceededMessage> _fakeProducer;
    private readonly IUserProfileLifecycleService _fakeLifecycleService;
    private readonly Fixture _fixture = new();
    private readonly DeleteUserProfileConsumer _sut;
    
    public DeleteUserProfileConsumerTest()
    {
        _fakeRepo = A.Fake<IUserProfileRepository>();
        _fakeUow = A.Fake<IUnitOfWork>();
        _fakeProducer = A.Fake<IProducer<OperationSucceededMessage>>();
        _fakeLifecycleService = A.Fake<IUserProfileLifecycleService>();
        var logger = A.Fake<ILogger<DeleteUserProfileConsumer>>();
        _sut = new DeleteUserProfileConsumer(_fakeRepo, _fakeLifecycleService, _fakeUow, _fakeProducer, logger);
    }

    [Fact]
    public async Task Consume_When_User_Exists_Should_SoftDelete()
    {
        // Arrange
        var message = _fixture.Create<UserDeleteMessage>();
        var user = new UserProfile();
        A.CallTo(() => _fakeRepo.GetByIdAsync(message.UserProfileId)).Returns(user);

        var fakeConsumeContext = A.Fake<ConsumeContext<UserDeleteMessage>>();
        A.CallTo(() => fakeConsumeContext.Message).Returns(message);

        // Act
        await _sut.Consume(fakeConsumeContext);

        // Assert
        A.CallTo(() => _fakeLifecycleService.SoftDelete(user)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeUow.SaveChangesAsync(A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeProducer.PublishMessageAsync(A<OperationSucceededMessage>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Consume_When_User_Does_Not_Exist_Should_Throw_KeyNotFoundException()
    {
        // Arrange
        var message = _fixture.Create<UserDeleteMessage>();
        A.CallTo(() => _fakeRepo.GetByIdAsync(message.UserProfileId)).Returns<UserProfile?>(null);

        var fakeConsumeContext = A.Fake<ConsumeContext<UserDeleteMessage>>();
        A.CallTo(() => fakeConsumeContext.Message).Returns(message);

        // Act
        var act = async () => await _sut.Consume(fakeConsumeContext);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }


}