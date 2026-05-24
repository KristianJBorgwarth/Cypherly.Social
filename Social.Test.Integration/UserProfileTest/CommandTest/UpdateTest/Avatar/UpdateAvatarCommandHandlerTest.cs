using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Social.Application.Contracts.Repositories;
using Social.Application.Contracts.Services;
using Social.Application.Features.UserProfile.Commands.Update.Avatar;
using Social.Domain.Aggregates;
using Social.Domain.ValueObjects;
using Social.Infrastructure.Persistence.Context;
using Social.Test.Integration.Setup;
using Social.Test.Integration.Setup.Helpers;

namespace Social.Test.Integration.UserProfileTest.CommandTest.UpdateTest.Avatar;

public class UpdateAvatarCommandHandlerTest : IntegrationTestBase
{
    private readonly UpdateAvatarCommandHandler _sut;
    public UpdateAvatarCommandHandlerTest(IntegrationTestFactory<Program, SocialDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var avatarService = scope.ServiceProvider.GetRequiredService<IAvatarService>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        _sut = new(repo, unitOfWork, avatarService);
    }

    [Fact]
    public async Task Handle_ValidUpdateCommand_Should_Update_ProfilePic_And_Return_Dto()
    {
        // Arrange
        var user = new UserProfile(Guid.NewGuid(), "TestUser", UserTag.Create("TestUser"));
        Db.UserProfile.Add(user);
        await Db.SaveChangesAsync();

        var command = new UpdateAvatarCommand()
        {
            TenantId = user.Id,
            NewProfilePicture = FormFileHelper.CreateFormFile(Path.Combine(DirectoryHelper.GetProjectRootDirectory(), "Social.Test.Integration/Setup/Resources/test_profile_picture.png"), "image/png")
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.RequiredValue.Etag.Should().NotBeNullOrEmpty();
        result.RequiredValue.AvatarId.Should().NotBeEmpty();
        var updatedUser = await Db.UserProfile.AsNoTracking().Include(userProfile => userProfile.Avatar).FirstOrDefaultAsync(x => x.Id == user.Id);
        updatedUser.Should().NotBeNull();
        updatedUser.Avatar.Should().NotBeNull();
    }
}