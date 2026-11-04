namespace URL_Shortener.RedirectApi.Infrastructure;

public record ReadLongUrlResponse(bool Found, string? LongUrl);