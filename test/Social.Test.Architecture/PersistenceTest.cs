using Social.Test.Architecture.Helpers;
using Social.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using NetArchTest.Rules;

namespace Social.Test.Architecture;

public class PersistenceTest
{
    [Fact]
    public void Infrastructure_Should_Not_Reference_Presentation()
    {
        var result = Types.InAssembly(typeof(SocialDbContext).Assembly)
            .That()
            .ResideInNamespace("Social.Persistence")
            .ShouldNot()
            .HaveDependencyOn("Social.API")
            .GetResult();

        result.ShouldBeSuccessful("Infrastructure project should not reference Presentation project");
    }

    [Fact]
    public void Infrastructure_Should_Not_Reference_Application_Outside_Of_Repositories()
    {
        var result = Types.InAssembly(typeof(SocialDbContext).Assembly)
            .That()
            .ResideInNamespace("Social.Identity.Persistence")
            .And()
            .DoNotResideInNamespace("Social.Infrastructure.Persistence.Extensions")
            .And()
            .DoNotResideInNamespace("Social.Infrastructure.Persistence.Repositories")
            .ShouldNot()
            .HaveDependencyOn("Social.Application")
            .GetResult();

        result.ShouldBeSuccessful("Infrastructure project should not reference Application project outside of Repositories and Configration");
    }

    [Fact]
    public void All_Repositories_Should_Reference_Domain()
    {
        var result = Types.InAssembly(typeof(SocialDbContext).Assembly)
            .That()
            .HaveNameEndingWith("Repository")
            .And().DoNotHaveNameEndingWith("OutboxRepository")
            .Should()
            .HaveDependencyOn("Social.Domain")
            .GetResult();

        result.ShouldBeSuccessful("All repositories should reference Domain project");
    }

    [Fact]
    public void All_Repositories_Should_Reference_Infrastructure_Context()
    {
        var result = Types.InAssembly(typeof(SocialDbContext).Assembly)
            .That()
            .HaveNameEndingWith("Repository")
            .And().DoNotHaveNameEndingWith("OutboxRepository")
            .Should()
            .HaveDependencyOn("Social.Infrastructure.Persistence.Context")
            .GetResult();

        result.ShouldBeSuccessful("All repositories should reference Infrastructure Context project");
    }

    [Fact]
    public void All_Contexts_Should_Inherit_From_CypherlyDbContext()
    {
        var result = Types.InAssembly(typeof(SocialDbContext).Assembly)
            .That()
            .HaveNameEndingWith("DbContext")
            .And().AreClasses()
            .Should()
            .Inherit(typeof(DbContext))
            .GetResult();

        result.ShouldBeSuccessful("All contexts should inherit from CypherlyDbContext");
    }

    [Fact]
    public void All_ModelConfigurations_Should_Inherit_From_IEntityTypeConfiguration()
    {
        var result = Types.InAssembly(typeof(SocialDbContext).Assembly)
            .That()
            .AreClasses()
            .And()
            .ResideInNamespace("Cypherly.Identity.Persistence.ModelConfiguration")
            .Should()
            .ImplementInterface(typeof(IEntityTypeConfiguration<>))
            .GetResult();

        result.ShouldBeSuccessful("All ModelConfigurations should inherit from IEntityTypeConfiguration");
    }
}