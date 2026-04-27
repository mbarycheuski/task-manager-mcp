using System.ComponentModel;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;
using TaskManager.Mcp.Common;
using TaskManager.Mcp.Common.Services;
using TaskManager.Mcp.Services;

namespace TaskManager.Mcp.Prompts;

[McpServerPromptType]
public class TaskPrompts(IPromptService promptService, ITimeService timeService)
{
    [McpServerPrompt(Name = "daily-plan")]
    [Description(
        "Builds a daily planning prompt for the top 3 highest-priority tasks due on the given date (defaults to today)."
    )]
    public async Task<ChatMessage> GetDailyPlanAsync(
        [Description($"The date to plan for in {DateFormats.Default} format. Defaults to today.")]
            string? date = null,
        CancellationToken cancellationToken = default
    )
    {
        var planDate = DateOnly.TryParseExact(date, DateFormats.Default, out var parsed)
            ? parsed
            : timeService.GetTodayInDefaultTimezone();
        var promptText = await promptService.GetDailyPlanAsync(planDate, cancellationToken);

        return new ChatMessage(ChatRole.User, promptText);
    }

    [McpServerPrompt(Name = "prioritize-tasks")]
    [Description("Reviews all open tasks and suggests a prioritized order to work through them.")]
    public async Task<ChatMessage> GetPrioritizeTasksAsync(CancellationToken cancellationToken)
    {
        var promptText = await promptService.GetPrioritizeTasksAsync(cancellationToken);

        return new ChatMessage(ChatRole.User, promptText);
    }
}
