using System;
using System.Net.Http;
using AutoFixture;
using Social.Infrastructure.Persistence.Context;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

// ReSharper disable MemberCanBeProtected.Global

namespace Social.Test.Integration.Setup;

[Collection("UserManagementApplication")]
public class IntegrationTestBase : IDisposable
{
    protected readonly SocialDbContext Db;
    protected readonly HttpClient Client;
    protected readonly ITestHarness Harness;
    protected readonly Fixture Fixture;

    public IntegrationTestBase(IntegrationTestFactory<Program, SocialDbContext> factory)
    {
        Fixture = new Fixture();
        Harness = factory.Services.GetTestHarness();
        var scope = factory.Services.CreateScope();
        Db = scope.ServiceProvider.GetRequiredService<SocialDbContext>();
        Db.Database.EnsureCreated();
        Client = factory.CreateClient();
        Harness.Start();
    }

    public void Dispose()
    {
        Db.Friendship.ExecuteDelete();
        Db.BlockedUser.ExecuteDelete();
        Db.UserProfile.ExecuteDelete();
        Db.OutboxMessage.ExecuteDelete();
    }
}