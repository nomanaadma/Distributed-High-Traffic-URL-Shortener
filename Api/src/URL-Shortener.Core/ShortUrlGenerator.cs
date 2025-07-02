namespace URL_Shortener.Core;

public class ShortUrlGenerator(TokenProvider tokenProvider)
{
	public object GenerateUniqueUrl()
	{
		return tokenProvider.GetToken().EncodeToBase62();
	}
}