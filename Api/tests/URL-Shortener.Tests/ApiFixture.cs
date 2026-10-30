using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using URL_Shortener.Api;
using URL_Shortener.Core.Urls.Add;
using Url_Shortener.Tests;
using URL_Shortener.Tests.TestDoubles;
using UrlShortener.Libraries.Testing.Extensions;

namespace URL_Shortener.Tests;

public class ApiFixture : WebApplicationFactory<IApiAssemblyMarker>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(
            services =>
            {
                services.Remove<IUrlDataStore>();
                services
                    .AddSingleton<IUrlDataStore>(
                        new InMemoryUrlDataStore());
                
                services.Remove<ITokenRangeApiClient>();
                services.AddSingleton<ITokenRangeApiClient, FakeTokenRangeApiClient>();
                
                services.AddAuthentication(defaultScheme: "TestScheme")
	                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
		                "TestScheme", options => { });
                
                services.AddAuthorizationBuilder()
                    .SetDefaultPolicy(new AuthorizationPolicyBuilder()
	                .RequireAuthenticatedUser()
	                .Build())
                    .SetFallbackPolicy(null);
                
            }
        );
        
        base.ConfigureWebHost(builder);
    }
}
