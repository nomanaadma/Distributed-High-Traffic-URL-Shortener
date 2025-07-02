using URL_Shortener.Core;

namespace URL_Shortener.Api.Core.Tests;

public class ShortUrlGeneratorScenarios
{
	// TEST LIST
	// 1. Check if the end range is greater than the start range
	// 2. Unique
	// 3. Accept multiple ranges
	// 4. Range as a type
	
	[Fact]
	public void Should_return_short_url_for_10001()
	{
		var tokenProvider = new TokenProvider();
		tokenProvider.AssignRange(10001, 20000);
		var shortUrlGenerator = new ShortUrlGenerator(tokenProvider);

		var shortUrl = shortUrlGenerator.GenerateUniqueUrl();
        
		shortUrl.Should().Be("2bJ");
	}
    
	[Fact]
	public void Should_return_short_url_for_zero()
	{
		var tokenProvider = new TokenProvider();
		tokenProvider.AssignRange(0, 10);
		var shortUrlGenerator = new ShortUrlGenerator(tokenProvider);

		var shortUrl = shortUrlGenerator.GenerateUniqueUrl();
        
		shortUrl.Should().Be("0");
	}
}