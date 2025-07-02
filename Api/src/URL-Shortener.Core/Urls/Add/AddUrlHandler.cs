namespace URL_Shortener.Core.Urls.Add;

public class AddUrlHandler(ShortUrlGenerator shortUrlGenerator, IUrlDataStore urlDataStore)
{
	public async Task<AddUrlResponse> HandleAsync(AddUrlRequest request, CancellationToken cancellationToken)
	{

		var shortened = new ShortenedUrl(
			LongUrl: request.LongUrl,
			ShortUrl: shortUrlGenerator.GenerateUniqueUrl()
		);
		
		await urlDataStore.AddAsync(shortened, cancellationToken);
		
		return new AddUrlResponse(request.LongUrl, shortened.ShortUrl);

	}

}