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
    private readonly GetUserProfileQueryHandler _sut;

    public GetUserProfileQueryHandlerTest()
    {
        _fakeRepository = A.Fake<IUserProfileRepository>();
        _fakeProfilePictureService = A.Fake<IProfilePictureService>();
        var fakeLogger = A.Fake<ILogger<GetUserProfileQueryHandler>>();
        _sut = new(_fakeRepository, _fakeProfilePictureService, fakeLogger);
    }

    [Fact]
    public async Task Handle_WhenUserProfileDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var query = new GetUserProfileQuery { TenantId = Guid.NewGuid(), ExclusiveConnectionId = Guid.NewGuid() };
        A.CallTo(() => _fakeRepository.GetByIdAsync(query.TenantId)).Returns((UserProfile)null!);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Code.Should().Be("entity.not.found");

        A.CallTo(() => _fakeRepository.GetByIdAsync(query.TenantId)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeProfilePictureService.GetPresignedProfilePictureUrlAsync(A<string>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenExceptionOccurs_ReturnsUnspecifiedError()
    {
        // Arrange
        var query = new GetUserProfileQuery { TenantId = Guid.NewGuid(), ExclusiveConnectionId = Guid.NewGuid()};
        A.CallTo(() => _fakeRepository.GetByIdAsync(query.TenantId)).Throws<Exception>();

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Contain("An exception occured while handling the request");
        A.CallTo(() => _fakeRepository.GetByIdAsync(query.TenantId)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeProfilePictureService.GetPresignedProfilePictureUrlAsync(A<string>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenUserProfileExists_ReturnsUserProfile()
    {
        // Arrange
        var connectionId = Guid.NewGuid();
        var query = new GetUserProfileQuery { TenantId = Guid.NewGuid(), ExclusiveConnectionId = Guid.NewGuid() };

        var userProfile = new UserProfile(Guid.NewGuid(), "David", UserTag.Create("David"));

        A.CallTo(() => _fakeRepository.GetByIdAsync(query.TenantId)).Returns(userProfile);

        var dto = GetUserProfileDto.MapFrom(userProfile, "");

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(dto);
        A.CallTo(() => _fakeRepository.GetByIdAsync(query.TenantId)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async void Handle_WhenUserProfileExists_And_Has_ProfilePicture_ReturnsUserProfile_With_ProfilePictureUrl()
    {
        // Arrange
        var connectionId = Guid.NewGuid();
        var query = new GetUserProfileQuery { TenantId = Guid.NewGuid(), ExclusiveConnectionId = Guid.NewGuid()};

        var userProfile = new UserProfile(Guid.NewGuid(), "David", UserTag.Create("David"));
        userProfile.SetProfilePictureUrl("profilePictureUrl");

        A.CallTo(() => _fakeRepository.GetByIdAsync(query.TenantId)).Returns(userProfile);
        A.CallTo(() => _fakeProfilePictureService.GetPresignedProfilePictureUrlAsync(userProfile.ProfilePictureUrl)).Returns(Result.Ok("presignedUrl"));


        var dto = GetUserProfileDto.MapFrom(userProfile, "presignedUrl");


        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.ProfilePictureUrl.Should().Be("presignedUrl");
        A.CallTo(() => _fakeProfilePictureService.GetPresignedProfilePictureUrlAsync(userProfile.ProfilePictureUrl)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeRepository.GetByIdAsync(query.TenantId)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async void Handle_When_ProfilePictureServiceFails_Should_Return_UserProfile_With_EmptyUrl()
    {
        // Arrange
        var query = new GetUserProfileQuery { TenantId = Guid.NewGuid(), ExclusiveConnectionId = Guid.NewGuid()};

        var userProfile = new UserProfile(Guid.NewGuid(), "David", UserTag.Create("David"));
        userProfile.SetProfilePictureUrl("profilePictureUrl");

        A.CallTo(() => _fakeRepository.GetByIdAsync(query.TenantId)).Returns(userProfile);

        A.CallTo(() => _fakeProfilePictureService.GetPresignedProfilePictureUrlAsync(userProfile.ProfilePictureUrl))
            .Returns(Result.Fail<string>(Errors.General.UnspecifiedError("Failed to get presigned url")));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.ProfilePictureUrl.Should().Be("");
        A.CallTo(() => _fakeProfilePictureService.GetPresignedProfilePictureUrlAsync(userProfile.ProfilePictureUrl)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeRepository.GetByIdAsync(query.TenantId)).MustHaveHappenedOnceExactly();
    }
}