using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace URL_Shortener.CosmosDbTriggerFunction
{
    public class ShortUrlPropagation(ILoggerFactory loggerFactory, Container container, IConnectionMultiplexer redis)
    {
	    private readonly ILogger _logger = loggerFactory.CreateLogger<ShortUrlPropagation>();

        [Function("ShortUrlPropagation")]
        public async Task Run([CosmosDBTrigger(
                databaseName: "urls",
                containerName: "items",
                Connection = "CosmosDbConnection",
                LeaseContainerName = "leases",
                CreateLeaseContainerIfNotExists = true)]
            IReadOnlyList<UrlDocument> input)
        {
            if (input is not { Count: > 0 }) return;
            
            foreach (var document in input)
            {
                _logger.LogInformation("Short Url: {ShortUrl}", document.Id);
                
                if (document.IsDeleted)
                {
	                try
	                {
		                await container.DeleteItemAsync<object>(document.Id,
			                new PartitionKey(document.CreatedBy));
	                }
	                catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
	                {
		                // Already gone (e.g. the trigger retried); nothing to do.
	                }

	                await redis.GetDatabase().KeyDeleteAsync(document.Id);
	                continue;
                }
                
                try
                {
                    var cosmosDbDocument = new ShortenedUrlEntity(
                        document.LongUrl,
                        document.Id,
                        document.CreatedOn,
                        document.CreatedBy
                    );
                    await container.UpsertItemAsync(cosmosDbDocument, new PartitionKey(document.CreatedBy));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error writing to Cosmos DB");
                    throw;
                }
            }
        }
    }

    public class UrlDocument
    {
	    public string Id { get; set; } = string.Empty;
        public DateTimeOffset CreatedOn { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string LongUrl { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
    }
    
    public class ShortenedUrlEntity(
	    string longUrl,
	    string shortUrl,
	    DateTimeOffset createdOn,
	    string createdBy)
    {
        public string LongUrl { get; } = longUrl;

        [JsonProperty(PropertyName = "id")] // Cosmos DB Unique Identifier
        public string ShortUrl { get; } = shortUrl;

        public DateTimeOffset CreatedOn { get; } = createdOn;

        [JsonProperty(PropertyName = "PartitionKey")] // Cosmos DB Partition Key
        public string CreatedBy { get; } = createdBy;
    }
}