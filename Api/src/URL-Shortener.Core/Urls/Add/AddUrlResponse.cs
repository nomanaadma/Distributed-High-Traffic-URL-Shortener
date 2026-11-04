namespace URL_Shortener.Core.Urls.Add;

public record AddUrlResponse(string Id, Uri LongUrl, Uri ShortUrl);