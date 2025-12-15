using Social.Application.Contracts.Repositories;
using Social.Application.Contracts.Services;
using Social.Application.Features.UserProfile.Commands.Update.ProfilePicture;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Social.Domain.Aggregates;
using Social.Domain.Common;
using Social.Domain.ValueObjects;
using Xunit;

namespace Social.Test.Unit.UserProfileTest.CommandTest.UpdateTest.ProfilePicture;

public class UpdateUserProfilePictuerCommandHandler
{
    private readonly IUserProfileRepository _fakeRepo;
    private readonly IProfilePictureService _fakeProfilePicService;
    private readonly IUnitOfWork _fakeUow;
    private readonly UpdateUserProfilePictureCommandHandler _sut;

    public UpdateUserProfilePictuerCommandHandler()
    {
        _fakeRepo = A.Fake<IUserProfileRepository>();
        _fakeProfilePicService = A.Fake<IProfilePictureService>();
        _fakeUow = A.Fake<IUnitOfWork>();
        var logger = A.Fake<ILogger<UpdateUserProfilePictureCommandHandler>>();
        _sut = new(_fakeRepo, _fakeProfilePicService, _fakeUow, logger);
    }

    [Fact]
    public async void Handle_GivenValidCommand_ShouldUpdateProfilePicture_AndReturn_UpdatedUser()
    {
        // Arrange
        var command = new UpdateUserProfilePictureCommand
        {
            TenantId = Guid.NewGuid(),
            NewProfilePicture = A.Fake<IFormFile>()
        };

        var userProfile = new UserProfile(Guid.NewGuid(), "test", UserTag.Create("test"));

        A.CallTo(() => _fakeRepo.GetByIdAsync(command.TenantId)).Returns(userProfile);

        A.CallTo(() => _fakeProfilePicService.UploadProfilePictureAsync(command.NewProfilePicture, command.TenantId))
            .Returns(Result.Ok("somestring"));

        A.CallTo(() => _fakeUow.SaveChangesAsync(A<System.Threading.CancellationToken>._)).DoesNothing();

        A.CallTo(() => _fakeProfilePicService.GetPresignedProfilePictureUrlAsync("somestring"))
            .Returns(Result.Ok("somestring"));

        // Act
        var result = await _sut.Handle(command, new());

        // Assert
        result.Success.Should().BeTrue();
        result.Value.ProfilePictureUrl.Should().Be("somestring");
    }

    [Fact]
    public async void Handle_Given_Invalid_Id_Should_Not_Update_ProfilePicture_And_Return_Error()
    {
        // Arrange
        var command = new UpdateUserProfilePictureCommand()
        {
            TenantId = Guid.NewGuid(),
            NewProfilePicture = A.Fake<IFormFile>()
        };

        A.CallTo(() => _fakeRepo.GetByIdAsync(command.TenantId))!.Returns<UserProfile>(null!);

        // Act
        var result = await _sut.Handle(command, new());

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Code.Should().Be("entity.not.found");
    }

    [Fact]
    public async void Handle_Given_Invalid_ProfilePicture_Should_Not_Update_ProfilePicture_And_Return_Error()
    {
        // Arrange
        var command = new UpdateUserProfilePictureCommand()
        {
            TenantId = Guid.NewGuid(),
            NewProfilePicture = A.Fake<IFormFile>()
        };

        var userProfile = new UserProfile(Guid.NewGuid(), "test", UserTag.Create("test"));

        A.CallTo(() => _fakeRepo.GetByIdAsync(command.TenantId)).Returns(userProfile);
        A.CallTo(() => _fakeProfilePicService.UploadProfilePictureAsync(command.NewProfilePicture, command.TenantId))
            .Returns(Result.Fail<string>(Errors.General.UnspecifiedError("Invalid profile picture")));

        // Act
        var result = await _sut.Handle(command, new());

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Code.Should().Be("unspecified.error");
        result.Error.Message.Should().Be("Invalid profile picture");
    }
}
