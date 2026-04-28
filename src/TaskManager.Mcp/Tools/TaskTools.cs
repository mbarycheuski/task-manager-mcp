using System.ComponentModel;
using ModelContextProtocol.Server;
using TaskManager.Mcp.Common;
using TaskManager.Mcp.Contracts;
using TaskManager.Mcp.Contracts.Inputs;
using TaskManager.Mcp.Exceptions;
using TaskManager.Mcp.Services;
using TaskItemOutput = TaskManager.Mcp.Contracts.Outputs.TaskItem;

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
            IReadOnlyList<TaskItemStatus>? statuses = null,
        [Description(
            $"Filter to tasks with due date on or after this date, in {DateFormats.Default} format (optional, inclusive)"
        )]
            DateOnly? dueDateFrom = null,
        [Description(
            $"Filter to tasks with due date on or before this date, in {DateFormats.Default} format (optional, inclusive). To match a single date, set dueDateFrom and dueDateTo to the same value."
        )]
            DateOnly? dueDateTo = null,
        CancellationToken cancellationToken = default
    )
    {
        if (statuses is not null && statuses.Any(s => !Enum.IsDefined(s)))
            throw new ValidationException("One or more status values are invalid.");

        if (dueDateFrom.HasValue && dueDateTo.HasValue && dueDateFrom.Value > dueDateTo.Value)
            throw new ValidationException("dueDateFrom cannot be later than dueDateTo.");

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
            throw new ValidationException("Task ID cannot be empty.");

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
        [Description("Task notes (optional)")] string? notes = null,
        [Description("Task priority: Low, Medium, High, or Critical (optional)")]
            TaskPriority? priority = null,
        [Description(
            $"Due date in {DateFormats.Default} format (optional, must be today or later)"
        )]
            DateOnly? dueDate = null,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ValidationException("Title is required and cannot be empty.");

        if (title.Length > ValidationConstants.MaxTitleLength)
            throw new ValidationException(
                $"Title cannot exceed {ValidationConstants.MaxTitleLength} characters."
            );

        if (notes is not null && notes.Length > ValidationConstants.MaxNotesLength)
            throw new ValidationException(
                $"Notes cannot exceed {ValidationConstants.MaxNotesLength} characters."
            );

        if (priority.HasValue && !Enum.IsDefined(priority.Value))
            throw new ValidationException("Priority is not a valid value.");

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
        [Description("Updated status: None, InProgress, or Completed (required)")] string status,
        [Description("Updated task notes (optional)")] string? notes = null,
        [Description("Updated priority: Low, Medium, High, or Critical (optional)")]
            TaskPriority? priority = null,
        [Description(
            $"Updated due date in {DateFormats.Default} format (optional, must be today or later)"
        )]
            DateOnly? dueDate = null,
        CancellationToken cancellationToken = default
    )
    {
        if (id == Guid.Empty)
            throw new ValidationException("Task ID cannot be empty.");

        if (string.IsNullOrWhiteSpace(title))
            throw new ValidationException("Title is required and cannot be empty.");

        if (title.Length > ValidationConstants.MaxTitleLength)
            throw new ValidationException(
                $"Title cannot exceed {ValidationConstants.MaxTitleLength} characters."
            );

        if (notes is not null && notes.Length > ValidationConstants.MaxNotesLength)
            throw new ValidationException(
                $"Notes cannot exceed {ValidationConstants.MaxNotesLength} characters."
            );

        if (priority.HasValue && !Enum.IsDefined(priority.Value))
            throw new ValidationException("Priority is not a valid value.");

        if (
            string.IsNullOrWhiteSpace(status)
            || !Enum.TryParse<TaskItemStatus>(status, ignoreCase: true, out var parsedStatus)
            || !Enum.IsDefined(parsedStatus)
        )
            throw new ValidationException("Status is not a valid value.");

        var input = new UpdateTaskInput(title, notes, priority, parsedStatus, dueDate);

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
            throw new ValidationException("Task ID cannot be empty.");

        return taskService.DeleteAsync(id, cancellationToken);
    }
}
