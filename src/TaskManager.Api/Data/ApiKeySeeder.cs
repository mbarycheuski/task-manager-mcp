using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Auth;
using TaskManager.Api.Data.Models;

namespace TaskManager.Api.Data;

public class ApiKeySeeder(
    TaskDbContext dbContext,
    IApiKeyHasher apiKeyHasher,
    IConfiguration configuration,
    ILogger<ApiKeySeeder> logger
)
{
    private const string ApiKeyConfigKey = "API_KEY";
    private const string DefaultClientName = "mcp-server";
    private const int SaltByteLength = 32;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await dbContext.ApiKeys.AnyAsync(cancellationToken))
        {
            return;
        }

        var rawKey = configuration[ApiKeyConfigKey];

        if (string.IsNullOrWhiteSpace(rawKey))
        {
            logger.LogWarning(
                "{Key} is not configured. Skipping API key seeding.",
                ApiKeyConfigKey
            );

            return;
        }

        var saltBytes = RandomNumberGenerator.GetBytes(SaltByteLength);
        var salt = Convert.ToBase64String(saltBytes);
        var keyHash = apiKeyHasher.HashKey(rawKey, salt);

        dbContext.ApiKeys.Add(
            new ApiKey
            {
                ClientName = DefaultClientName,
                KeyHash = keyHash,
                Salt = salt,
                IsActive = true,
            }
        );

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation($"Seeded default API key for client '{DefaultClientName}'.");
    }
}
