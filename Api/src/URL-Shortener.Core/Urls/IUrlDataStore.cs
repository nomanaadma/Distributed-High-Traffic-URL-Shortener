using URL_Shortener.Core.Urls.Delete;

namespace URL_Shortener.Core.Urls;

public interface IUrlDataStore
{
    Task AddAsync(ShortenedUrl shortened, CancellationToken cancellationToken);
    Task<DeleteUrlResponse> DeleteAsync(string shortUrl, string requestedBy, CancellationToken cancellationToken);
}