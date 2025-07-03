using Microsoft.AspNetCore.Mvc.Testing;
using URL_Shortener.Api;

namespace URL_Shortener.Tests;

public class ApiFixture : WebApplicationFactory<IApiAssemblyMarker>
{
    // protected override void ConfigureWebHost(IWebHostBuilder builder)
    // {
    //     builder.ConfigureTestServices(
    //         services =>
    //         {
    //             services.Remove<IUrlDataStore>();
    //             services
    //                 .AddSingleton<IUrlDataStore>(
    //                     new InMemoryUrlDataStore());
    //             
    //             services.Remove<ITokenRangeApiClient>();
    //             services.AddSingleton<ITokenRangeApiClient, FakeTokenRangeApiClient>();
    //         }
    //     );
    //     
    //     base.ConfigureWebHost(builder);
    // }
}