using Social.Infrastructure.Persistence.Context;

namespace Social.Test.Integration.Setup.Fixtures;

[CollectionDefinition("UserManagementApplication")]
public class FactoryTestFixture : ICollectionFixture<IntegrationTestFactory<Program, SocialDbContext>>
{

}