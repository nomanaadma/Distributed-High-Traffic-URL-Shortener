namespace URL_Shortener.Core;

public class TokenProvider
{
	private long _start;
	private TokenRange? _tokenRange;
	public void AssignRange(long start, long end)
	{
		_tokenRange = new TokenRange(start, end);
		_start = _tokenRange.Start;
	}
	
	public void AssignRange(TokenRange tokenRange)
	{
		_start = tokenRange.Start;
	}
	
	public long GetToken()
	{
		return _tokenRange.Start;
	}
}