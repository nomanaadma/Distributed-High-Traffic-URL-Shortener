namespace URL_Shortener.Core.Urls.List;

public record ListUrlsRequest(string Author, int? PageSize, string? ContinuationToken = null);