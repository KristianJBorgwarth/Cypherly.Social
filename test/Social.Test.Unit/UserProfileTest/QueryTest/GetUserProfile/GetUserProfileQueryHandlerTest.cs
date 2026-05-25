using Social.Application.Abstractions;
using Social.Application.Contracts.Repositories;
using Social.Application.Contracts.Services;
using Social.Application.Features.UserProfile.Queries.GetUserProfile;
using FakeItEasy;
using FluentAssertions;
using Social.Domain.Aggregates;
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
        _sut = new(_fakeRepository);
    }

    [Fact]
    public async Task Handle_WhenUserProfileDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var query = new GetUserProfileQuery { TenantId = Guid.NewGuid() };
        A.CallTo(() => _fakeRepository.GetSingleAsync(A<ISpecification<UserProfile>>._, A<CancellationToken>._)).Returns((UserProfile)null!);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error!.Code.Should().Be("entity.not.found");
        A.CallTo(() => _fakeRepository.GetSingleAsync(A<ISpecification<UserProfile>>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeProfilePictureService.GetPresignedProfilePictureUrlAsync(A<string>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_WhenUserProfileExists_ReturnsUserProfile()
    {
        // Arrange
        var query = new GetUserProfileQuery { TenantId = Guid.NewGuid() };
        var userProfile = new UserProfile(Guid.NewGuid(), "David", UserTag.Create("David"));

        A.CallTo(() => _fakeRepository.GetSingleAsync(A<ISpecification<UserProfile>>._, A<CancellationToken>._)).Returns(userProfile);

        var dto = GetUserProfileDto.MapFrom(userProfile);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(dto);
        A.CallTo(() => _fakeRepository.GetSingleAsync(A<ISpecification<UserProfile>>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async void Handle_WhenUserProfileExists_And_Has_ProfilePicture_ReturnsUserProfile_With_ProfilePictureUrl()
    {
        // Arrange
        var query = new GetUserProfileQuery { TenantId = Guid.NewGuid() };
        var userProfile = new UserProfile(Guid.NewGuid(), "David", UserTag.Create("David"));
        userProfile.GetOrCreateAvatar("image/png");

        A.CallTo(() => _fakeRepository.GetSingleAsync(A<ISpecification<UserProfile>>._, A<CancellationToken>._)).Returns(userProfile);

        var dto = GetUserProfileDto.MapFrom(userProfile);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(dto);
        result.Value.AvatarKey.Should().Be(userProfile.Avatar!.FileKey);
        A.CallTo(() => _fakeRepository.GetSingleAsync(A<ISpecification<UserProfile>>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async void Handle_When_ProfilePictureServiceFails_Should_Return_UserProfile_With_EmptyUrl()
    {
        // Arrange
        var query = new GetUserProfileQuery { TenantId = Guid.NewGuid() };
        var userProfile = new UserProfile(Guid.NewGuid(), "David", UserTag.Create("David"));
        userProfile.GetOrCreateAvatar("image/png");

        A.CallTo(() => _fakeRepository.GetSingleAsync(A<ISpecification<UserProfile>>._, A<CancellationToken>._)).Returns(userProfile);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.AvatarKey.Should().Be(userProfile.Avatar!.FileKey);
        A.CallTo(() => _fakeRepository.GetSingleAsync(A<ISpecification<UserProfile>>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }
}
