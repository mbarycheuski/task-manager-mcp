using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using TaskManager.Api.Repositories;

namespace TaskManager.Api.Auth;

public class ApiKeyAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IApiKeyRepository apiKeyRepository,
    IApiKeyCacheService apiKeyCacheService,
    IApiKeyHasher apiKeyHasher
) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(ApiKeyDefaults.HeaderName, out var apiKeyValues))
        {
            return AuthenticateResult.NoResult();
        }

        var rawKey = apiKeyValues.FirstOrDefault();

        if (string.IsNullOrEmpty(rawKey))
        {
            return AuthenticateResult.NoResult();
        }

        var cachedClientName = apiKeyCacheService.GetApiClient(rawKey);

        if (cachedClientName is not null)
        {
            return CreateSuccessResult(cachedClientName);
        }

        var activeKeys = await apiKeyRepository.GetActiveKeysAsync(Context.RequestAborted);

        var matchedApiKey = activeKeys.SingleOrDefault(k =>
            apiKeyHasher.Verify(rawKey, k.Salt, k.KeyHash)
        );

        if (matchedApiKey is null)
        {
            return AuthenticateResult.Fail("Invalid API key.");
        }

        apiKeyCacheService.StoreApiClient(rawKey, matchedApiKey.ClientName);

        return CreateSuccessResult(matchedApiKey.ClientName);
    }

    private AuthenticateResult CreateSuccessResult(string clientName)
    {
        var claims = new[] { new Claim(ClaimTypes.Name, clientName) };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}
