using Microsoft.AspNetCore.Mvc.Testing;
using URL_Shortener.RedirectApi.Infrastructure;
using URL_Shortener.RedirectApi.Tests.TestDoubles;
using System.Net;

namespace URL_Shortener.RedirectApi.Tests;

[Collection("Api collection")]
public class RedirectScenarios(ApiFixture fixture)
{
    private readonly HttpClient _client = fixture.CreateClient(new WebApplicationFactoryClientOptions()
    {
	    AllowAutoRedirect = false
    });
    private readonly InMemoryShortenedUrlReader _storage = fixture.ShortenedUrlReader;

    [Fact]
    public async Task Should_return_301_redirect_with_Url_when_short_url_exits()
    {
        const string shortUrl = "abc123";
        _storage.Add(shortUrl, new ReadLongUrlResponse(true, "https://google.com"));
        
        var response = await _client.GetAsync($"/r/{shortUrl}");
        
        response.Should()
	        .HaveHttpStatusCode(HttpStatusCode.Redirect);
        
        response.Headers.Location.Should().Be("https://google.com");
    }
    
    [Fact]
    public async Task Should_return_404_not_found_when_short_url_does_not_exist()
    {
        const string shortUrl = "non-existing";
        
        var response = await _client.GetAsync($"/r/{shortUrl}");
        
        response.Should()
            .HaveHttpStatusCode(System.Net.HttpStatusCode.NotFound);
    }
}











