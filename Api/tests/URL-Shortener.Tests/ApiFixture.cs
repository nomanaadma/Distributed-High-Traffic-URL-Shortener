using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using URL_Shortener.Api;
using URL_Shortener.Core.Urls.Add;
using URL_Shortener.Core.Urls.List;
using Url_Shortener.Tests;
using URL_Shortener.Tests.TestDoubles;
using URL_Shortener.Libraries.Testing.Extensions;

namespace URL_Shortener.Tests;

public class ApiFixture : WebApplicationFactory<IApiAssemblyMarker>
{
	public ApiFixture()
	{
		Environment.SetEnvironmentVariable("RedirectService__Endpoint", "https://urlshortener.tests/r/");
	}
	
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(
            services =>
            {
	            var inMemoryStore = new InMemoryUrlDataStore();
                services.Remove<IUrlDataStore>();
                services
                    .AddSingleton<IUrlDataStore>(
	                    inMemoryStore);
                
                services.Remove<IUserUrlsReader>();
                services
	                .AddSingleton<IUserUrlsReader>(
		                inMemoryStore);
                
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
