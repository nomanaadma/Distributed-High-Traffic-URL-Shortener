using URL_Shortener.Core.Urls;
using URL_Shortener.Core.Urls.List;

namespace URL_Shortener.Tests.TestDoubles;

public class InMemoryUrlDataStore : Dictionary<string, ShortenedUrl>, IUrlDataStore, IUserUrlsReader
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

    public Task<UserUrls> GetAsync(string createdBy,
	    int pageSize,
	    string? continuationToken,
	    CancellationToken cancellationToken)
    {
	    var data = Values
		    .Where(u => u.CreatedBy == createdBy)
		    .Select((u, index) => (index, new UserUrlItem(u.ShortUrl, u.LongUrl.ToString(), u.CreatedOn)))
		    .Where(entry => continuationToken == null || entry.index > int.Parse(continuationToken))
		    .Take(pageSize)
		    .ToList();

	    var urls = data.Select(entry => entry.Item2);
	    var lastItemIndex = data.Last().index;
	    
	    return Task.FromResult(new UserUrls(urls,
		    lastItemIndex.ToString()));
    }
}