namespace URL_Shortener.Core;

public class TokenProvider
{
	private int _start;
	public void AssignRange(int start, int end)
	{
		_start = start;
	}
	
	public int GetToken()
	{
		return _start;
	}
}