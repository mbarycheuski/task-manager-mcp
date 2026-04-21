namespace TaskManager.Api.Auth;

public interface IApiKeyCacheService
{
    string? GetApiClient(string rawKey);
    void StoreApiClient(string rawKey, string clientName);
}
