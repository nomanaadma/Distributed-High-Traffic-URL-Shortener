namespace URL_Shortener.Core;

public record TokenRange
{
	public TokenRange(int start, int end)
	{
		
		if (end < start)
			throw new ArgumentException("End must be greater than or equal to Start");
		
		Start = start;
		End = end;
	}
	public int Start { get; }
	public int End { get; }
}