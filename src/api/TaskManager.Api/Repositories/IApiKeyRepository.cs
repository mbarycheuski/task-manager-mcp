using TaskManager.Api.Data.Models;

namespace TaskManager.Api.Repositories;

public interface IApiKeyRepository
{
    Task<IReadOnlyList<ApiKey>> GetActiveKeysAsync(CancellationToken cancellationToken);
}
