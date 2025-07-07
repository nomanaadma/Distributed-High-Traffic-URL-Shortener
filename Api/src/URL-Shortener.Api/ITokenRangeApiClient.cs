using URL_Shortener.Core;

public interface ITokenRangeApiClient
{
    Task<TokenRange?> AssignRangeAsync(string machineKey, CancellationToken cancellationToken);
}