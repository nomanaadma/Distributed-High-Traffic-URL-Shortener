namespace URL_Shortener.Core.Urls.Delete;

public record DeleteUrlRequest(string shortUrl, string requestedBy);