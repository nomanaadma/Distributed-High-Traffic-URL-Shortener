using URL_Shortener.Core.Urls;
using URL_Shortener.Core.Urls.Add;

namespace URL_Shortener.Api.Core.Tests.TestDoubles;

public class InMemoryUrlDataStore : Dictionary<string, ShortenedUrl>, IUrlDataStore
{
    public Task AddAsync(ShortenedUrl shortened, CancellationToken cancellationToken)
    {
        Add(shortened.ShortUrl, shortened);
        return Task.CompletedTask;
    }

    public Task<DeleteUrlResult> DeleteAsync(string shortUrl, string requestedBy,
	    CancellationToken cancellationToken)
    {
	    var deleted = TryGetValue(shortUrl, out var url)
          && string.Equals(url.CreatedBy, requestedBy, StringComparison.OrdinalIgnoreCase)
          && Remove(shortUrl);

	    return Task.FromResult(deleted ? DeleteUrlResult.Deleted : DeleteUrlResult.NotFound);
    }
}