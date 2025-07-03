using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using URL_Shortener.Api;
using URL_Shortener.Core.Urls;
using URL_Shortener.Core.Urls.Add;
using URL_Shortener.Tests.Extensions;

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
                
                // services.Remove<ITokenRangeApiClient>();
                // services.AddSingleton<ITokenRangeApiClient, FakeTokenRangeApiClient>();
            }
        );
        
        base.ConfigureWebHost(builder);
    }
}

public class InMemoryUrlDataStore : Dictionary<string, ShortenedUrl>, IUrlDataStore
{
    public Task AddAsync(ShortenedUrl shortened, CancellationToken cancellationToken)
    {
        Add(shortened.ShortUrl, shortened);
        return Task.CompletedTask;
    }
}