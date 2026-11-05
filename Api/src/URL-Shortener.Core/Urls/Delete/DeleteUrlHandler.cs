namespace URL_Shortener.Core.Urls.Delete;

public class DeleteUrlHandler(IUrlDataStore urlDataStore)
{
	public async Task<Result<DeleteUrlResponse>> HandleAsync(DeleteUrlRequest request, CancellationToken cancellationToken)
	{
		return await urlDataStore.DeleteAsync(request.shortUrl, request.requestedBy, cancellationToken);
	}
}