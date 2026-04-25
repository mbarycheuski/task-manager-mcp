using System.ComponentModel;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using TaskManager.Mcp.Services;
using TaskManager.Mcp.Tools;
using TaskManager.Mcp.Utilities;
using TaskManager.Mcp.Utilities.Serializers;

namespace TaskManager.Mcp.Resources;

[McpServerResourceType]
public class TaskResources(ITaskService taskService, IOutputSerializer serializer)
{
    [McpServerResource(UriTemplate = "tasks://all", Name = "all-tasks", MimeType = MediaTypes.Json)]
    [Description("All tasks.")]
    public async Task<string> GetAllTasksAsync(CancellationToken cancellationToken)
    {
        var tasks = await McpErrorHandler.ExecuteAsync(() =>
            taskService.GetAllAsync(cancellationToken)
        );

        return serializer.Serialize(tasks);
    }

    [McpServerResource(
        UriTemplate = "tasks://completed",
        Name = "completed-tasks",
        MimeType = MediaTypes.Json
    )]
    [Description("Tasks with status Completed.")]
    public async Task<string> GetCompletedTasksAsync(CancellationToken cancellationToken)
    {
        var tasks = await McpErrorHandler.ExecuteAsync(() =>
            taskService.GetCompletedAsync(cancellationToken)
        );

        return serializer.Serialize(tasks);
    }

    [McpServerResource(
        UriTemplate = "tasks://in-progress",
        Name = "in-progress-tasks",
        MimeType = MediaTypes.Json
    )]
    [Description("Tasks with status InProgress.")]
    public async Task<string> GetInProgressTasksAsync(CancellationToken cancellationToken)
    {
        var tasks = await McpErrorHandler.ExecuteAsync(() =>
            taskService.GetInProgressAsync(cancellationToken)
        );

        return serializer.Serialize(tasks);
    }

    [McpServerResource(
        UriTemplate = "tasks://today",
        Name = "today-tasks",
        MimeType = MediaTypes.Json
    )]
    [Description("Tasks due today.")]
    public async Task<string> GetTodayTasksAsync(CancellationToken cancellationToken)
    {
        var tasks = await McpErrorHandler.ExecuteAsync(() =>
            taskService.GetTodayAsync(cancellationToken)
        );

        return serializer.Serialize(tasks);
    }

    [McpServerResource(UriTemplate = "tasks://{id}", Name = "task", MimeType = MediaTypes.Json)]
    [Description("A single task by ID.")]
    public async Task<string> GetTaskAsync(Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
            throw new McpProtocolException("Task ID cannot be empty.", McpErrorCode.InvalidParams);

        var task = await McpErrorHandler.ExecuteAsync(() =>
            taskService.GetByIdAsync(id, cancellationToken)
        );

        return serializer.Serialize(task);
    }
}
