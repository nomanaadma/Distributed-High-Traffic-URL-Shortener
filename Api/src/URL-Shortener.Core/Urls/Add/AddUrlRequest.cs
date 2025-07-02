namespace URL_Shortener.Core.Urls.Add;

public record AddUrlRequest(Uri LongUrl, string CreatedBy);