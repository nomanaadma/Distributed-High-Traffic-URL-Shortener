namespace URL_Shortener.Core.Urls.Add;

public class AddUrlHandler(
		ShortUrlGenerator shortUrlGenerator, 
		IUrlDataStore urlDataStore,
		TimeProvider timeProvider,
		RedirectLinkBuilder redirectLinkBuilder
	)
{
	public async Task<Result<AddUrlResponse>> HandleAsync(AddUrlRequest request, CancellationToken cancellationToken)
	{

		if (string.IsNullOrEmpty(request.CreatedBy))
			return Errors.MissingCreatedBy;
		
		var shortened = new ShortenedUrl(
			LongUrl: request.LongUrl,
			ShortUrl: shortUrlGenerator.GenerateUniqueUrl(),
			CreatedBy: request.CreatedBy,
			CreatedOn: timeProvider.GetUtcNow()
		);
		
		await urlDataStore.AddAsync(shortened, cancellationToken);
		
		return new AddUrlResponse(
			shortened.ShortUrl,
			request.LongUrl, 
			redirectLinkBuilder.LinkTo(shortened.ShortUrl));
		
	}

}