using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using URL_Shortener.Core.Urls;

namespace URL_Shortener.Infrastructure;

public class CosmosDbUrlDataStore(Container container) : IUrlDataStore
{
	public async Task AddAsync(ShortenedUrl shortened, CancellationToken cancellationToken)
    {
        var document = (ShortenedUrlCosmos)shortened;
        await container.CreateItemAsync(document,
            new PartitionKey(document.PartitionKey),
            cancellationToken: cancellationToken);
    }

    internal class ShortenedUrlCosmos(
        string longUrl, 
        string shortUrl, 
        string createdBy, 
        DateTimeOffset createdOn)
    {
        [JsonProperty(PropertyName = "longUrl")]
        public string LongUrl { get; } = longUrl;

        [JsonProperty(PropertyName = "id")] // Cosmos DB Unique Identifier
        public string ShortUrl { get; } = shortUrl;

        [JsonProperty(PropertyName = "createdOn")]
        public DateTimeOffset CreatedOn { get; } = createdOn;

        [JsonProperty(PropertyName = "createdBy")]
        public string CreatedBy { get; } = createdBy;

        public string PartitionKey => ShortUrl[..1]; // Cosmos DB Partition Key

        public static implicit operator ShortenedUrl(ShortenedUrlCosmos url) =>
            new(new Uri(url.LongUrl), url.ShortUrl, url.CreatedBy, url.CreatedOn);

        public static explicit operator ShortenedUrlCosmos(ShortenedUrl url) =>
            new(url.LongUrl.ToString(), url.ShortUrl, url.CreatedBy, url.CreatedOn);
    }
    
    private const int SoftDeleteTtlSeconds = 3600;
    
    public async Task<DeleteUrlResult> DeleteAsync(string shortUrl, string requestedBy,
	    CancellationToken cancellationToken)
    {
	    var partitionKey = new PartitionKey(shortUrl[..1]);

	    OwnerRecord owner;
	    try
	    {
		    owner = (await container.ReadItemAsync<OwnerRecord>(shortUrl, partitionKey,
			    cancellationToken: cancellationToken)).Resource;
	    }
	    catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
	    {
		    return DeleteUrlResult.NotFound;
	    }

	    if (owner.IsDeleted ||
	        !string.Equals(owner.CreatedBy, requestedBy, StringComparison.OrdinalIgnoreCase))
		    return DeleteUrlResult.NotFound;

	    await container.PatchItemAsync<OwnerRecord>(shortUrl, partitionKey,
		    [
			    PatchOperation.Set("/isDeleted", true),
			    PatchOperation.Set("/ttl", SoftDeleteTtlSeconds)
		    ],
		    cancellationToken: cancellationToken);

	    return DeleteUrlResult.Deleted;
    }

    internal record OwnerRecord(string CreatedBy, bool IsDeleted);
}