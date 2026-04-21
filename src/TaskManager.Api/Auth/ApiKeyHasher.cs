using System.Security.Cryptography;
using System.Text;

namespace TaskManager.Api.Auth;

public class ApiKeyHasher : IApiKeyHasher
{
    public string HashKey(string rawKey, string salt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rawKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(salt);

        var saltBytes = Convert.FromBase64String(salt);
        var keyBytes = Encoding.UTF8.GetBytes(rawKey);
        using var hmac = new HMACSHA256(saltBytes);
        var hashBytes = hmac.ComputeHash(keyBytes);

        return Convert.ToBase64String(hashBytes);
    }

    public bool Verify(string rawKey, string salt, string storedHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rawKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(salt);
        ArgumentException.ThrowIfNullOrWhiteSpace(storedHash);

        var computedHash = HashKey(rawKey, salt);

        return CryptographicOperations.FixedTimeEquals(
            Convert.FromBase64String(computedHash),
            Convert.FromBase64String(storedHash)
        );
    }
}
