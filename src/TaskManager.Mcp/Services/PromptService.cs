using SmartFormat;
using TaskManager.Mcp.Common;
using TaskManager.Mcp.Prompts;
using TaskManager.Mcp.Providers;

namespace TaskManager.Mcp.Services;

public class PromptService(IPromptProvider promptProvider) : IPromptService
{
    public async Task<string> GetDailyPlanAsync(DateOnly date, CancellationToken cancellationToken)
    {
        var promptTemplate = await promptProvider.GetAsync(
            PromptResources.DailyPlan,
            cancellationToken
        );
        var context = new { Date = date.ToString(DateFormats.Default) };

        return Smart.Format(promptTemplate, context);
    }

    public async Task<string> GetPrioritizeTasksAsync(CancellationToken cancellationToken) =>
        await promptProvider.GetAsync(PromptResources.PrioritizeTasks, cancellationToken);
}
