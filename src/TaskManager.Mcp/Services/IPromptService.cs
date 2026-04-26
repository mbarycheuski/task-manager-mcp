namespace TaskManager.Mcp.Services;

public interface IPromptService
{
    Task<string> GetDailyPlanAsync(DateOnly date, CancellationToken cancellationToken);

    Task<string> GetPrioritizeTasksAsync(CancellationToken cancellationToken);
}
