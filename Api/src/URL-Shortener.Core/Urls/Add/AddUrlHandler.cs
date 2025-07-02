namespace URL_Shortener.Core.Urls.Add;

public class AddUrlHandler(
		ShortUrlGenerator shortUrlGenerator, 
		IUrlDataStore urlDataStore,
		TimeProvider timeProvider
	)
{
	public async Task<AddUrlResponse> HandleAsync(AddUrlRequest request, CancellationToken cancellationToken)
	{

		var shortened = new ShortenedUrl(
			LongUrl: request.LongUrl,
			ShortUrl: shortUrlGenerator.GenerateUniqueUrl(),
			CreatedBy: request.CreatedBy,
			CreatedOn: timeProvider.GetUtcNow()
		);
		
		await urlDataStore.AddAsync(shortened, cancellationToken);
		
		return new AddUrlResponse(request.LongUrl, shortened.ShortUrl);

	}

}