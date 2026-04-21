namespace TaskManager.Api.Auth;

public interface IApiKeyHasher
{
    string HashKey(string rawKey, string salt);
    bool Verify(string rawKey, string salt, string storedHash);
}
