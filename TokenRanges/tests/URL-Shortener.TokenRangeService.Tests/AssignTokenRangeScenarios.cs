using System.Collections.Concurrent;
using System.Net.Http.Json;
using AwesomeAssertions;

namespace URL_Shortener.TokenRangeService.Tests;

public class AssignTokenRangeScenarios(Fixture fixture) : IClassFixture<Fixture>
{
    private readonly HttpClient _client = fixture.CreateClient();
    
    [Fact]
    public async Task Should_return_range_when_requested()
    {
        var requestResponse = await AssignTokenPostAsync("tests");

        requestResponse.Should().Be2XXSuccessful();
        
        var tokenRange = await requestResponse.Content
            .ReadFromJsonAsync<TokenRangeResponse>();
        tokenRange!.Start.Should().BeGreaterThan(0);
        tokenRange.End.Should().BeGreaterThan(tokenRange.Start);
    }

    [Fact]
    public async Task Should_not_repeat_range_when_requested()
    {
        var requestResponse1 = await AssignTokenPostAsync("tests");
        var requestResponse2 = await AssignTokenPostAsync("tests");
        
        requestResponse1.Should().Be2XXSuccessful();
        requestResponse2.Should().Be2XXSuccessful();
        
        var tokenRange1 = await requestResponse1.Content
            .ReadFromJsonAsync<TokenRangeResponse>();
        var tokenRange2 = await requestResponse2.Content
            .ReadFromJsonAsync<TokenRangeResponse>();
        tokenRange2!.Start.Should().BeGreaterThan(tokenRange1!.End);
    }
    
    
    [Fact]
    public async Task Should_not_repeat_range_on_multiple_requests()
    {
        ConcurrentBag<TokenRangeResponse> ranges = [];
        await Parallel.ForEachAsync(Enumerable.Range(1, 100), async (number, cancellationToken) =>
        {
            var response = await AssignTokenPostAsync(number.ToString());
            
            if (response.IsSuccessStatusCode)
            {
                var range = await response.Content
                    .ReadFromJsonAsync<TokenRangeResponse>(cancellationToken: cancellationToken);
                ranges.Add(range!);
            }
        });
    
        ranges.Should().OnlyHaveUniqueItems(x => x.Start);
        ranges.Should().OnlyHaveUniqueItems(x => x.End);
    }
    
    private async Task<HttpResponseMessage> AssignTokenPostAsync(string key)
    {
        return await _client.PostAsJsonAsync("/assign",
            new AssignTokenRangeRequest(key));
    }
}


