using Social.Infrastructure.Persistence.Context;
using Social.Test.Integration.Setup;

namespace Social.Test.Integration.UserProfileTest.EndpointTest;

public class GetAvatarEndpointTest : IntegrationTestBase
{
    private readonly HttpClient _client;

    public GetAvatarEndpointTest(IntegrationTestFactory<Program, SocialDbContext> factory) : base(factory)
    {

    }
}
