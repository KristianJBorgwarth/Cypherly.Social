using AutoFixture;
using Cypherly.Message.Contracts.Messages.Profile;
using Cypherly.Message.Contracts.Responses.Profile;
using Social.Application.Contracts.Repositories;
using Social.Application.Features.UserProfile.Consumers;
using FakeItEasy;
using MassTransit;
using Microsoft.Extensions.Logging;
using Social.Domain.Aggregates;
using Social.Domain.Services;
using Social.Domain.ValueObjects;
using Xunit;

namespace Social.Test.Unit.UserProfileTest.ConsumerTest
{
    public class CreateUserProfileConsumerTest
    {
        private readonly IUserProfileRepository _fakeUserProfileRepo;
        private readonly IUnitOfWork _fakeUnitOfWork;
        private readonly IUserProfileLifecycleService _fakeUserProfileLifecycleService;
        private readonly CreateUserProfileConsumer _sut;
        private readonly Fixture _fixture = new();

        public CreateUserProfileConsumerTest()
        {
            _fakeUserProfileRepo = A.Fake<IUserProfileRepository>();
            _fakeUnitOfWork = A.Fake<IUnitOfWork>();
            _fakeUserProfileLifecycleService = A.Fake<IUserProfileLifecycleService>();
            var fakeLogger = A.Fake<ILogger<CreateUserProfileConsumer>>();
            _sut = new(_fakeUserProfileRepo, _fakeUnitOfWork, _fakeUserProfileLifecycleService, fakeLogger);
        }

        [Fact]
        public async Task Consume_WhenCalled_ShouldCreateUserProfile()
        {
            // Arrange
            var message = _fixture.Build<CreateUserProfileMessage>()
                .With(x => x.Username, "testUser")
                .Create();
            
            var profile = new UserProfile(message.UserId, message.Username, UserTag.Create(message.Username));

            A.CallTo(() => _fakeUserProfileLifecycleService.CreateUserProfile(message.UserId, message.Username)).Returns(profile);

            A.CallTo(() => _fakeUserProfileRepo.CreateAsync(profile)).Returns(Task.CompletedTask);

            A.CallTo(() => _fakeUnitOfWork.SaveChangesAsync(CancellationToken.None)).Returns(Task.CompletedTask);

            var fakeConsumeContext = A.Fake<ConsumeContext<CreateUserProfileMessage>>();
            A.CallTo(() => fakeConsumeContext.Message).Returns(message);

            // Act
            await _sut.Consume(fakeConsumeContext);

            // Assert
            A.CallTo(() => _fakeUserProfileLifecycleService.CreateUserProfile(message.UserId, message.Username)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeUserProfileRepo.CreateAsync(profile)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _fakeUnitOfWork.SaveChangesAsync(CancellationToken.None)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Consume_WhenServiceThrowsException_ShouldRespondWithError()
        {
            // Arrange
            var message = _fixture.Build<CreateUserProfileMessage>()
                .With(x => x.Username, "testUser")
                .Create();

            var fakeConsumeContext = A.Fake<ConsumeContext<CreateUserProfileMessage>>();
            A.CallTo(() => fakeConsumeContext.Message).Returns(message);

            A.CallTo(() => _fakeUserProfileLifecycleService.CreateUserProfile(message.UserId, message.Username))
                .Throws(new Exception("Service exception"));

            // Act
            await Assert.ThrowsAsync<Exception>(() => _sut.Consume(fakeConsumeContext));

            // Assert
            A.CallTo(() => fakeConsumeContext.RespondAsync(A<CreateUserProfileResponse>.That.Matches(r => r.IsSuccess == false))).MustHaveHappened();
        }

        [Fact]
        public async Task Consume_WhenRepositoryThrowsException_ShouldRespondWithError()
        {
            // Arrange
            var message = _fixture.Build<CreateUserProfileMessage>()
                .With(x => x.Username, "testUser")
                .Create();
            
            var profile = new UserProfile(message.UserId, message.Username, UserTag.Create(message.Username));

            var fakeConsumeContext = A.Fake<ConsumeContext<CreateUserProfileMessage>>();
            A.CallTo(() => fakeConsumeContext.Message).Returns(message);

            A.CallTo(() => _fakeUserProfileLifecycleService.CreateUserProfile(message.UserId, message.Username)).Returns(profile);
            A.CallTo(() => _fakeUserProfileRepo.CreateAsync(profile)).Throws(new Exception("Repository exception"));

            // Act
            await Assert.ThrowsAsync<Exception>(() => _sut.Consume(fakeConsumeContext));

            // Assert
            A.CallTo(() => fakeConsumeContext.RespondAsync(A<CreateUserProfileResponse>.That.Matches(r => r.IsSuccess == false))).MustHaveHappened();
        }

        [Fact]
        public async Task Consume_WhenUnitOfWorkThrowsException_ShouldRespondWithError()
        {
            // Arrange
            var message = _fixture.Build<CreateUserProfileMessage>()
                .With(x => x.Username, "testUser")
                .Create();
            
            var profile = new UserProfile(message.UserId, message.Username, UserTag.Create(message.Username));

            var fakeConsumeContext = A.Fake<ConsumeContext<CreateUserProfileMessage>>();
            A.CallTo(() => fakeConsumeContext.Message).Returns(message);

            A.CallTo(() => _fakeUserProfileLifecycleService.CreateUserProfile(message.UserId, message.Username)).Returns(profile);
            A.CallTo(() => _fakeUserProfileRepo.CreateAsync(profile)).Returns(Task.CompletedTask);
            A.CallTo(() => _fakeUnitOfWork.SaveChangesAsync(CancellationToken.None)).Throws(new Exception("UnitOfWork exception"));

            // Act
            await Assert.ThrowsAsync<Exception>(() => _sut.Consume(fakeConsumeContext));

            // Assert
            A.CallTo(() => fakeConsumeContext.RespondAsync(A<CreateUserProfileResponse>.That.Matches(r => r.IsSuccess == false))).MustHaveHappened();
        }
    }
}
