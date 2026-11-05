using URL_Shortener.Core.Urls;
using URL_Shortener.Core.Urls.Add;
using URL_Shortener.Core.Urls.Delete;

namespace URL_Shortener.Api.Core.Tests.TestDoubles;

public class InMemoryUrlDataStore : Dictionary<string, ShortenedUrl>, IUrlDataStore
{
    public Task AddAsync(ShortenedUrl shortened, CancellationToken cancellationToken)
    {
        Add(shortened.ShortUrl, shortened);
        return Task.CompletedTask;
    }

    public Task<DeleteUrlResponse> DeleteAsync(string shortUrl, string requestedBy,
	    CancellationToken cancellationToken)
    {
	    var deleted = TryGetValue(shortUrl, out var url)
          && string.Equals(url.CreatedBy, requestedBy, StringComparison.OrdinalIgnoreCase)
          && Remove(shortUrl);

	    return Task.FromResult(deleted ? DeleteUrlResponse.Deleted : DeleteUrlResponse.NotFound);
    }
}