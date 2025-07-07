using Microsoft.Extensions.Logging;
using NSubstitute;
using URL_Shortener.Core;
using Url_Shortener.Api;

namespace URL_Shortener.Tests;

public class TokenManagerScenarios
{
    [Fact]
    public async Task Should_call_api_on_start()
    {
        var tokenRangeApiClient = Substitute.For<ITokenRangeApiClient>();
        tokenRangeApiClient.AssignRangeAsync(Arg.Any<string>(), CancellationToken.None)
            .Returns(new TokenRange(1, 10));
        var tokenManager = new TokenManager(tokenRangeApiClient,
            Substitute.For<ILogger<TokenManager>>(),
            Substitute.For<TokenProvider>(),
            Substitute.For<IEnvironmentManager>());

        await tokenManager.StartAsync(CancellationToken.None);

        await tokenRangeApiClient.Received()
            .AssignRangeAsync(Arg.Any<string>(), CancellationToken.None);
    }
    
    [Fact]
    public async Task Should_throw_exception_when_no_tokens_assigned()
    {
        var tokenRangeApiClient = Substitute.For<ITokenRangeApiClient>();
        var environmentManager = Substitute.For<IEnvironmentManager>();
        var tokenManager = new TokenManager(tokenRangeApiClient,
            Substitute.For<ILogger<TokenManager>>(),
            Substitute.For<TokenProvider>(),
            environmentManager
            );

        await tokenManager.StartAsync(CancellationToken.None);

        environmentManager.Received().FatalError();
    }
}