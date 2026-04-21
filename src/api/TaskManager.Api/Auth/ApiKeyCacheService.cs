using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using TaskManager.Api.Settings;

namespace TaskManager.Api.Auth;

public class ApiKeyCacheService(IMemoryCache memoryCache, IOptions<AppSettings> appSettings)
    : IApiKeyCacheService
{
    private readonly TimeSpan _slidingExpiration = appSettings.Value.ApiKeyCacheSlidingExpiration;

    public string? GetApiClient(string rawKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rawKey);

        var key = GetCacheKey(rawKey);

        return memoryCache.TryGetValue(key, out string? clientName) ? clientName : null;
    }

    public void StoreApiClient(string rawKey, string clientName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rawKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(clientName);

        var key = GetCacheKey(rawKey);

        memoryCache.Set(
            key,
            clientName,
            new MemoryCacheEntryOptions { SlidingExpiration = _slidingExpiration }
        );
    }

    private static string GetCacheKey(string rawKey)
    {
        var keyBytes = Encoding.UTF8.GetBytes(rawKey);
        var hashBytes = SHA256.HashData(keyBytes);

        return Convert.ToHexString(hashBytes);
    }
}
