using Social.Application.Contracts.Clients;
using FakeItEasy;
using FluentAssertions;
using Social.Domain.Common;
using Xunit;
using Social.Application.Features.UserProfile.Queries.GetAvatar;

namespace Social.Test.Unit.UserProfileTest.QueryTest.GetUserProfilePictureTest;

public class GetUserProfilePictureQueryHandlerTest
{
    private readonly IMinioProxyClient _fakeMinioProxyClient;
    private readonly GetAvatarQueryHandler _sut;

    public GetUserProfilePictureQueryHandlerTest()
    {
        _fakeMinioProxyClient = A.Fake<IMinioProxyClient>();
        _sut = new GetAvatarQueryHandler(_fakeMinioProxyClient);
    }

    [Fact]
    public async Task Handle_Valid_Query_Should_Return_GetUserProfilePictureDto()
    {
        // Arrange
        var query = new GetAvatarQuery { AvatarId = "https://example.com/profile.jpg" };

        A.CallTo(() => _fakeMinioProxyClient.GetImageFromMinioAsync(A<string>._, A<CancellationToken>._))
            .Returns(Result.Ok((new byte[] { 0x00 }, "image/jpeg")));

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Invalid_Query_Should_Return_Fail_Result()
    {
        // Arrange
        var query = new GetAvatarQuery { AvatarId = "invalid_url" };

        A.CallTo(() => _fakeMinioProxyClient.GetImageFromMinioAsync(A<string>._, A<CancellationToken>._))
            .Returns(Task.FromResult<(byte[] image, string imageType)?>(null));


        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
    }
}
