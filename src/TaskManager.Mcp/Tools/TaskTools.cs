using System.ComponentModel;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using TaskManager.Mcp.Common;
using TaskManager.Mcp.Inputs;
using TaskManager.Mcp.Services;
using TaskItemOutput = TaskManager.Mcp.Outputs.TaskItem;

namespace TaskManager.Mcp.Tools;

[McpServerToolType]
public class TaskTools(ITaskService taskService)
{
    [McpServerTool(
        Name = "get_all_tasks",
        UseStructuredContent = true,
        OutputSchemaType = typeof(TaskItemOutput[])
    )]
    [Description(
        "Returns tasks, optionally filtered by one or more statuses and/or due-date range. All filters are optional; omit them to return every task."
    )]
    public Task<IReadOnlyList<TaskItemOutput>> GetAllAsync(
        [Description(
            "Filter by one or more statuses: None, InProgress, Completed (optional). Multiple values match any of the given statuses."
        )]
            IReadOnlyList<Outputs.TaskItemStatus>? statuses,
        [Description(
            $"Filter to tasks with due date on or after this date, in {DateFormats.Default} format (optional, inclusive)"
        )]
            DateOnly? dueDateFrom,
        [Description(
            $"Filter to tasks with due date on or before this date, in {DateFormats.Default} format (optional, inclusive). To match a single date, set dueDateFrom and dueDateTo to the same value."
        )]
            DateOnly? dueDateTo,
        CancellationToken cancellationToken
    )
    {
        if (statuses is not null && statuses.Any(s => !Enum.IsDefined(s)))
            throw new McpProtocolException(
                "One or more status values are invalid.",
                McpErrorCode.InvalidParams
            );

        if (dueDateFrom.HasValue && dueDateTo.HasValue && dueDateFrom.Value > dueDateTo.Value)
            throw new McpProtocolException(
                "dueDateFrom cannot be later than dueDateTo.",
                McpErrorCode.InvalidParams
            );

        return taskService.GetAllAsync(statuses, dueDateFrom, dueDateTo, cancellationToken);
    }

    [McpServerTool(
        Name = "get_task",
        UseStructuredContent = true,
        OutputSchemaType = typeof(TaskItemOutput)
    )]
    [Description("Get a single task by ID.")]
    public Task<TaskItemOutput> GetByIdAsync(
        [Description("The task ID (UUID)")] Guid id,
        CancellationToken cancellationToken
    )
    {
        if (id == Guid.Empty)
            throw new McpProtocolException("Task ID cannot be empty.", McpErrorCode.InvalidParams);

        return taskService.GetByIdAsync(id, cancellationToken);
    }

    [McpServerTool(
        Name = "add_task",
        UseStructuredContent = true,
        OutputSchemaType = typeof(TaskItemOutput)
    )]
    [Description("Create a new task.")]
    public Task<TaskItemOutput> AddTaskAsync(
        [Description("Task title (required)")] string title,
        [Description("Task notes (optional)")] string? notes,
        [Description("Task priority: Low, Medium, High, or Critical (optional)")]
            Outputs.TaskPriority? priority,
        [Description(
            $"Due date in {DateFormats.Default} format (optional, must be today or later)"
        )]
            DateOnly? dueDate,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new McpProtocolException(
                "Title is required and cannot be empty.",
                McpErrorCode.InvalidParams
            );

        if (title.Length > ValidationConstants.MaxTitleLength)
            throw new McpProtocolException(
                $"Title cannot exceed {ValidationConstants.MaxTitleLength} characters.",
                McpErrorCode.InvalidParams
            );

        if (notes?.Length > ValidationConstants.MaxNotesLength)
            throw new McpProtocolException(
                $"Notes cannot exceed {ValidationConstants.MaxNotesLength} characters.",
                McpErrorCode.InvalidParams
            );

        if (priority.HasValue && !Enum.IsDefined(priority.Value))
            throw new McpProtocolException(
                "Priority is not a valid value.",
                McpErrorCode.InvalidParams
            );

        var input = new CreateTaskInput(title, notes, priority, dueDate);

        return taskService.CreateAsync(input, cancellationToken);
    }

    [McpServerTool(
        Name = "update_task",
        UseStructuredContent = true,
        OutputSchemaType = typeof(TaskItemOutput)
    )]
    [Description("Update an existing task.")]
    public Task<TaskItemOutput> UpdateTaskAsync(
        [Description("The task ID (UUID)")] Guid id,
        [Description("Updated task title (required)")] string title,
        [Description("Updated task notes (optional)")] string? notes,
        [Description("Updated priority: Low, Medium, High, or Critical (optional)")]
            Outputs.TaskPriority? priority,
        [Description("Updated status: None, InProgress, or Completed (required)")]
            Outputs.TaskItemStatus status,
        [Description(
            $"Updated due date in {DateFormats.Default} format (optional, must be today or later)"
        )]
            DateOnly? dueDate,
        CancellationToken cancellationToken
    )
    {
        if (id == Guid.Empty)
            throw new McpProtocolException("Task ID cannot be empty.", McpErrorCode.InvalidParams);

        if (string.IsNullOrWhiteSpace(title))
            throw new McpProtocolException(
                "Title is required and cannot be empty.",
                McpErrorCode.InvalidParams
            );

        if (title.Length > ValidationConstants.MaxTitleLength)
            throw new McpProtocolException(
                $"Title cannot exceed {ValidationConstants.MaxTitleLength} characters.",
                McpErrorCode.InvalidParams
            );

        if (notes?.Length > ValidationConstants.MaxNotesLength)
            throw new McpProtocolException(
                $"Notes cannot exceed {ValidationConstants.MaxNotesLength} characters.",
                McpErrorCode.InvalidParams
            );

        if (priority.HasValue && !Enum.IsDefined(priority.Value))
            throw new McpProtocolException(
                "Priority is not a valid value.",
                McpErrorCode.InvalidParams
            );

        if (!Enum.IsDefined(status))
            throw new McpProtocolException(
                "Status is not a valid value.",
                McpErrorCode.InvalidParams
            );

        var input = new UpdateTaskInput(title, notes, priority, status, dueDate);

        return taskService.UpdateAsync(id, input, cancellationToken);
    }

    [McpServerTool(Name = "delete_task")]
    [Description("Delete a task.")]
    public Task DeleteTaskAsync(
        [Description("The task ID (UUID)")] Guid id,
        CancellationToken cancellationToken
    )
    {
        if (id == Guid.Empty)
            throw new McpProtocolException("Task ID cannot be empty.", McpErrorCode.InvalidParams);

        return taskService.DeleteAsync(id, cancellationToken);
    }
}
