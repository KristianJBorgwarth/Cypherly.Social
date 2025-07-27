using Social.Application.Contracts.Clients;
using Social.Application.Contracts.Repositories;
using Social.Application.Contracts.Services;
using Social.Application.Features.UserProfile.Queries.GetUserProfile;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Social.Domain.Aggregates;
using Social.Domain.Common;
using Social.Domain.ValueObjects;
using Xunit;

namespace Social.Test.Unit.UserProfileTest.QueryTest.GetUserProfile;

public class GetUserProfileQueryHandlerTest
{
    private readonly IUserProfileRepository _fakeRepository;
    private readonly IProfilePictureService _fakeProfilePictureService;
    private readonly IConnectionIdProvider _fakeConnectionIdProvider;
    private readonly GetUserProfileQueryHandler _sut;

    public GetUserProfileQueryHandlerTest()
    {
        _fakeRepository = A.Fake<IUserProfileRepository>();
        _fakeProfilePictureService = A.Fake<IProfilePictureService>();
        _fakeConnectionIdProvider = A.Fake<IConnectionIdProvider>();
        var fakeLogger = A.Fake<ILogger<GetUserProfileQueryHandler>>();
        _sut = new(_fakeRepository, _fakeProfilePictureService, _fakeConnectionIdProvider, fakeLogger);
    }

    [Fact]
    public async Task Handle_WhenUserProfileDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var query = new GetUserProfileQuery { UserProfileId = Guid.NewGuid(), ExlusiveConnectionId = Guid.NewGuid() };
        A.CallTo(() => _fakeRepository.GetByIdAsync(query.UserProfileId)).Returns((UserProfile)null!);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Code.Should().Be("entity.not.found");

        A.CallTo(() => _fakeRepository.GetByIdAsync(query.UserProfileId)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeProfilePictureService.GetPresignedProfilePictureUrlAsync(A<string>._)).MustNotHaveHappened();
        A.CallTo(() => _fakeConnectionIdProvider.GetConnectionIdsByUser(A<Guid>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenExceptionOccurs_ReturnsUnspecifiedError()
    {
        // Arrange
        var query = new GetUserProfileQuery { UserProfileId = Guid.NewGuid(), ExlusiveConnectionId = Guid.NewGuid()};
        A.CallTo(() => _fakeRepository.GetByIdAsync(query.UserProfileId)).Throws<Exception>();

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Contain("An exception occured while handling the request");
        A.CallTo(() => _fakeRepository.GetByIdAsync(query.UserProfileId)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeProfilePictureService.GetPresignedProfilePictureUrlAsync(A<string>._)).MustNotHaveHappened();
        A.CallTo(() => _fakeConnectionIdProvider.GetConnectionIdsByUser(A<Guid>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenUserProfileExists_ReturnsUserProfile()
    {
        // Arrange
        var connectionId = Guid.NewGuid();
        var query = new GetUserProfileQuery { UserProfileId = Guid.NewGuid(), ExlusiveConnectionId = Guid.NewGuid() };

        var userProfile = new UserProfile(Guid.NewGuid(), "David", UserTag.Create("David"));

        A.CallTo(() => _fakeRepository.GetByIdAsync(query.UserProfileId)).Returns(userProfile);
        A.CallTo(() => _fakeConnectionIdProvider.GetConnectionIdsByUser(query.UserProfileId, A<CancellationToken>.Ignored))
            .Returns([connectionId, query.ExlusiveConnectionId]);

        var dto = GetUserProfileDto.MapFrom(userProfile, "", [connectionId]);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(dto);
        A.CallTo(() => _fakeRepository.GetByIdAsync(query.UserProfileId)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async void Handle_WhenUserProfileExists_And_Has_ProfilePicture_ReturnsUserProfile_With_ProfilePictureUrl()
    {
        // Arrange
        var connectionId = Guid.NewGuid();
        var query = new GetUserProfileQuery { UserProfileId = Guid.NewGuid(), ExlusiveConnectionId = Guid.NewGuid()};

        var userProfile = new UserProfile(Guid.NewGuid(), "David", UserTag.Create("David"));
        userProfile.SetProfilePictureUrl("profilePictureUrl");

        A.CallTo(() => _fakeRepository.GetByIdAsync(query.UserProfileId)).Returns(userProfile);
        A.CallTo(() => _fakeProfilePictureService.GetPresignedProfilePictureUrlAsync(userProfile.ProfilePictureUrl)).Returns(Result.Ok("presignedUrl"));
        A.CallTo(() => _fakeConnectionIdProvider.GetConnectionIdsByUser(query.UserProfileId ,A<CancellationToken>.Ignored))
            .Returns([connectionId, query.ExlusiveConnectionId]);

        var dto = GetUserProfileDto.MapFrom(userProfile, "presignedUrl", [connectionId]);


        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.ProfilePictureUrl.Should().Be("presignedUrl");
        A.CallTo(() => _fakeProfilePictureService.GetPresignedProfilePictureUrlAsync(userProfile.ProfilePictureUrl)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeRepository.GetByIdAsync(query.UserProfileId)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeConnectionIdProvider.GetConnectionIdsByUser(query.UserProfileId, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async void Handle_When_ProfilePictureServiceFails_Should_Return_UserProfile_With_EmptyUrl()
    {
        // Arrange
        var query = new GetUserProfileQuery { UserProfileId = Guid.NewGuid(), ExlusiveConnectionId = Guid.NewGuid()};

        var userProfile = new UserProfile(Guid.NewGuid(), "David", UserTag.Create("David"));
        userProfile.SetProfilePictureUrl("profilePictureUrl");

        A.CallTo(() => _fakeRepository.GetByIdAsync(query.UserProfileId)).Returns(userProfile);

        A.CallTo(() => _fakeProfilePictureService.GetPresignedProfilePictureUrlAsync(userProfile.ProfilePictureUrl))
            .Returns(Result.Fail<string>(Errors.General.UnspecifiedError("Failed to get presigned url")));

        A.CallTo(() => _fakeConnectionIdProvider.GetConnectionIdsByUser(userProfile.Id, A<CancellationToken>.Ignored))
            .Returns([Guid.NewGuid(), query.ExlusiveConnectionId]);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.ProfilePictureUrl.Should().Be("");
        A.CallTo(() => _fakeProfilePictureService.GetPresignedProfilePictureUrlAsync(userProfile.ProfilePictureUrl)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeRepository.GetByIdAsync(query.UserProfileId)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeConnectionIdProvider.GetConnectionIdsByUser(query.UserProfileId, A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }
}