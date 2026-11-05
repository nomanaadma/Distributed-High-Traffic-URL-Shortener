namespace URL_Shortener.Core.Urls;

public enum DeleteUrlResult { Deleted, NotFound }

public interface IUrlDataStore
{
    Task AddAsync(ShortenedUrl shortened, CancellationToken cancellationToken);
    Task<DeleteUrlResult> DeleteAsync(string shortUrl, string requestedBy, CancellationToken cancellationToken);
}