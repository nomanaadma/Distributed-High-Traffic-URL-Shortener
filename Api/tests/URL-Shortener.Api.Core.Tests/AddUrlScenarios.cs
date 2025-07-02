using Microsoft.Extensions.Time.Testing;
using URL_Shortener.Api.Core.Tests.TestDoubles;
using URL_Shortener.Core;
using URL_Shortener.Core.Urls.Add;

namespace URL_Shortener.Api.Core.Tests;

public class AddUrlScenarios
{
	private readonly AddUrlHandler _handler;
	private readonly InMemoryUrlDataStore _urlDataStore = new();
	private readonly FakeTimeProvider _timeProvider = new();

	public AddUrlScenarios()
	{
		var tokenProvider = new TokenProvider();
		tokenProvider.AssignRange(1, 5);
		var shortUrlGenerator = new ShortUrlGenerator(tokenProvider);
		_handler = new AddUrlHandler(shortUrlGenerator, _urlDataStore, _timeProvider);
	}
	
	[Fact]
	public async Task Should_return_shortened_url()
	{
		var request = CreateAddUrlRequest();
		var response = await _handler.HandleAsync(request, CancellationToken.None); 
		
		response.ShortUrl.Should().NotBeNullOrEmpty();
		response.ShortUrl.Should().Be("1");
	}
	
	[Fact]
	public async Task Should_save_short_url()
	{
		var request = CreateAddUrlRequest();

		var response = await _handler.HandleAsync(request, CancellationToken.None);
		
		_urlDataStore.Should().ContainKey(response.ShortUrl);
	}
	
	[Fact]
	public async Task Should_save_short_url_with_created_by()
	{
		var request = CreateAddUrlRequest();

		var response = await _handler.HandleAsync(request, CancellationToken.None);
	
		_urlDataStore.Should().ContainKey(response.ShortUrl);
		_urlDataStore[response.ShortUrl].CreatedBy.Should().Be(request.CreatedBy);
	}
	
	[Fact]
	public async Task Should_save_short_url_with_created_by_and_created_on()
	{
		var request = CreateAddUrlRequest();

		var response = await _handler.HandleAsync(request, CancellationToken.None);
	
		_urlDataStore.Should().ContainKey(response.ShortUrl);
		_urlDataStore[response.ShortUrl].CreatedBy.Should().Be(request.CreatedBy);
		_urlDataStore[response.ShortUrl].CreatedOn.Should().Be(_timeProvider.GetUtcNow());
	}

	private static AddUrlRequest CreateAddUrlRequest()
	{
		return new AddUrlRequest(new Uri("https://example.com/long-url"), "nomanaadma@gmail.com");
	}
}