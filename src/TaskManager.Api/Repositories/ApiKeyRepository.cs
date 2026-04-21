using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data;
using TaskManager.Api.Data.Models;

namespace TaskManager.Api.Repositories;

public class ApiKeyRepository(TaskDbContext taskDbContext) : IApiKeyRepository
{
    public async Task<IReadOnlyList<ApiKey>> GetActiveKeysAsync(CancellationToken cancellationToken)
    {
        return await taskDbContext
            .ApiKeys.AsNoTracking()
            .Where(k => k.IsActive)
            .ToListAsync(cancellationToken);
    }
}
