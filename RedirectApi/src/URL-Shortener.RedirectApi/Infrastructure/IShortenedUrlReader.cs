namespace URL_Shortener.RedirectApi.Infrastructure;

public interface IShortenedUrlReader
{
    public Task<ReadLongUrlResponse> GetLongUrlAsync(string shortUrl, CancellationToken cancellationToken);
}