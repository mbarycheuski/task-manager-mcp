using TaskManager.Mcp.Collaborators.Dto;
using TaskManager.Mcp.Contracts.Inputs;
using TaskItemOutput = TaskManager.Mcp.Contracts.Outputs.TaskItem;
using TaskItemStatusOutput = TaskManager.Mcp.Contracts.TaskItemStatus;
using TaskPriorityOutput = TaskManager.Mcp.Contracts.TaskPriority;

namespace TaskManager.Mcp.Mappers;

public static class TaskItemMappingExtensions
{
    public static TaskItemOutput ToOutput(this TaskItemDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new(
            Id: dto.Id,
            Title: dto.Title,
            Status: dto.Status.ToOutput(),
            CreatedAt: dto.CreatedAt,
            UpdatedAt: dto.UpdatedAt,
            Notes: dto.Notes,
            Priority: dto.Priority.ToOutput(),
            DueDate: dto.DueDate,
            CompletedAt: dto.CompletedAt
        );
    }

    public static CreateTaskRequestDto ToDto(this CreateTaskInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        return new(
            Title: input.Title,
            Notes: input.Notes,
            Priority: input.Priority.ToDto(),
            DueDate: input.DueDate
        );
    }

    public static UpdateTaskRequestDto ToDto(this UpdateTaskInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        return new(
            Title: input.Title,
            Notes: input.Notes,
            Priority: input.Priority.ToDto(),
            Status: input.Status.ToDto(),
            DueDate: input.DueDate
        );
    }

    public static TaskPriorityDto? ToDto(this TaskPriorityOutput? priority)
    {
        if (priority is null)
            return null;

        return priority.Value switch
        {
            TaskPriorityOutput.Low => TaskPriorityDto.Low,
            TaskPriorityOutput.Medium => TaskPriorityDto.Medium,
            TaskPriorityOutput.High => TaskPriorityDto.High,
            TaskPriorityOutput.Critical => TaskPriorityDto.Critical,
            _ => throw new ArgumentOutOfRangeException(
                nameof(priority),
                priority,
                $"Not supported {nameof(TaskPriorityOutput)} value."
            ),
        };
    }

    public static TaskItemStatusDto ToDto(this TaskItemStatusOutput status) =>
        status switch
        {
            TaskItemStatusOutput.None => TaskItemStatusDto.None,
            TaskItemStatusOutput.InProgress => TaskItemStatusDto.InProgress,
            TaskItemStatusOutput.Completed => TaskItemStatusDto.Completed,
            _ => throw new ArgumentOutOfRangeException(
                nameof(status),
                status,
                $"Not supported {nameof(TaskItemStatusOutput)} value."
            ),
        };

    public static IReadOnlyList<TaskItemStatusDto>? ToDto(
        this IReadOnlyList<TaskItemStatusOutput>? statuses
    )
    {
        if (statuses is null || statuses.Count == 0)
            return null;

        return [.. statuses.Select(s => s.ToDto())];
    }

    private static TaskItemStatusOutput ToOutput(this TaskItemStatusDto status) =>
        status switch
        {
            TaskItemStatusDto.None => TaskItemStatusOutput.None,
            TaskItemStatusDto.InProgress => TaskItemStatusOutput.InProgress,
            TaskItemStatusDto.Completed => TaskItemStatusOutput.Completed,
            _ => throw new ArgumentOutOfRangeException(
                nameof(status),
                status,
                $"Not supported {nameof(TaskItemStatusDto)} value."
            ),
        };

    private static TaskPriorityOutput? ToOutput(this TaskPriorityDto? priority)
    {
        if (priority is null)
            return null;

        return priority.Value switch
        {
            TaskPriorityDto.Low => TaskPriorityOutput.Low,
            TaskPriorityDto.Medium => TaskPriorityOutput.Medium,
            TaskPriorityDto.High => TaskPriorityOutput.High,
            TaskPriorityDto.Critical => TaskPriorityOutput.Critical,
            _ => throw new ArgumentOutOfRangeException(
                nameof(priority),
                priority,
                $"Not supported {nameof(TaskPriorityOutput)} value."
            ),
        };
    }
}
