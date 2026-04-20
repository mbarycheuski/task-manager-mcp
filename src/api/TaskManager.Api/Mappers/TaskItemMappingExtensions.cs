using TaskManager.Api.Contracts;
using TaskManager.Api.Contracts.Enums;
using TaskItemDomain = TaskManager.Api.Data.Models.TaskItem;
using TaskItemStatusDomain = TaskManager.Api.Data.Models.Enums.TaskItemStatus;
using TaskPriorityDomain = TaskManager.Api.Data.Models.Enums.TaskPriority;

namespace TaskManager.Api.Mappers;

public static class TaskItemMappingExtensions
{
    public static TaskItem ToContract(this TaskItemDomain entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new(
            entity.Id,
            entity.Title,
            entity.Notes,
            entity.Priority.ToContract(),
            entity.Status.ToContract(),
            entity.DueDate,
            entity.CreatedAt,
            entity.UpdatedAt,
            entity.CompletedAt
        );
    }

    public static TaskItemDomain ToDomain(this CreateTaskRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new()
        {
            Title = request.Title,
            Notes = request.Notes,
            Priority = request.Priority.ToDomain(),
            Status = TaskItemStatusDomain.None,
            DueDate = request.DueDate,
        };
    }

    public static TaskItemStatus ToContract(this TaskItemStatusDomain status) =>
        status switch
        {
            TaskItemStatusDomain.None => TaskItemStatus.None,
            TaskItemStatusDomain.InProgress => TaskItemStatus.InProgress,
            TaskItemStatusDomain.Completed => TaskItemStatus.Completed,
            _ => throw new ArgumentOutOfRangeException(
                nameof(status),
                status,
                $"Not supported {nameof(TaskItemStatusDomain)} value."
            ),
        };

    private static TaskPriority? ToContract(this TaskPriorityDomain? priority)
    {
        if (priority is null)
            return null;

        return priority.Value switch
        {
            TaskPriorityDomain.Low => TaskPriority.Low,
            TaskPriorityDomain.Medium => TaskPriority.Medium,
            TaskPriorityDomain.High => TaskPriority.High,
            TaskPriorityDomain.Critical => TaskPriority.Critical,
            _ => throw new ArgumentOutOfRangeException(
                nameof(priority),
                priority,
                $"Not supported {nameof(TaskPriorityDomain)} value."
            ),
        };
    }

    public static TaskItemStatusDomain ToDomain(this TaskItemStatus status) =>
        status switch
        {
            TaskItemStatus.None => TaskItemStatusDomain.None,
            TaskItemStatus.InProgress => TaskItemStatusDomain.InProgress,
            TaskItemStatus.Completed => TaskItemStatusDomain.Completed,
            _ => throw new ArgumentOutOfRangeException(
                nameof(status),
                status,
                $"Not supported {nameof(TaskItemStatus)} value."
            ),
        };

    public static TaskItemStatusDomain? ToDomain(this TaskItemStatus? status)
    {
        if (status is null)
            return null;

        return status.Value switch
        {
            TaskItemStatus.None => TaskItemStatusDomain.None,
            TaskItemStatus.InProgress => TaskItemStatusDomain.InProgress,
            TaskItemStatus.Completed => TaskItemStatusDomain.Completed,
            _ => throw new ArgumentOutOfRangeException(
                nameof(status),
                status,
                $"Not supported {nameof(TaskItemStatus)} value."
            ),
        };
    }

    public static TaskPriorityDomain? ToDomain(this TaskPriority? priority)
    {
        if (priority is null)
            return null;

        return priority.Value switch
        {
            TaskPriority.Low => TaskPriorityDomain.Low,
            TaskPriority.Medium => TaskPriorityDomain.Medium,
            TaskPriority.High => TaskPriorityDomain.High,
            TaskPriority.Critical => TaskPriorityDomain.Critical,
            _ => throw new ArgumentOutOfRangeException(
                nameof(priority),
                priority,
                $"Not supported {nameof(TaskPriority)} value."
            ),
        };
    }
}
