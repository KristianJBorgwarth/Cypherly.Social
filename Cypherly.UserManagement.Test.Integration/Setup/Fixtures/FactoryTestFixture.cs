using Social.Infrastructure.Persistence.Context;

namespace Cypherly.UserManagement.Test.Integration.Setup.Fixtures;

[CollectionDefinition("UserManagementApplication")]
public class FactoryTestFixture : ICollectionFixture<IntegrationTestFactory<Program, UserManagementDbContext>>
{

}