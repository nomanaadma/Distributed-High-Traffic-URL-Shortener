using URL_Shortener.RedirectApi.Infrastructure;

namespace URL_Shortener.RedirectApi.Tests.TestDoubles;

public class InMemoryShortenedUrlReader : Dictionary<string, ReadLongUrlResponse>, IShortenedUrlReader
{
    public Task<ReadLongUrlResponse> GetLongUrlAsync(string shortUrl, CancellationToken cancellationToken)
    {
        return Task.FromResult(ContainsKey(shortUrl)? this[shortUrl]: new ReadLongUrlResponse(false, null));
    }
}