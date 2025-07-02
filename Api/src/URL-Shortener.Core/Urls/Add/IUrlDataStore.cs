namespace URL_Shortener.Core.Urls.Add;

public interface IUrlDataStore
{
    Task AddAsync(ShortenedUrl shortened, CancellationToken cancellationToken);
}