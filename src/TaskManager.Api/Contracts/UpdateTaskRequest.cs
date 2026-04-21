using TaskManager.Api.Contracts.Enums;

namespace TaskManager.Api.Contracts;

public record UpdateTaskRequest(
    string Title,
    string? Notes,
    TaskPriority? Priority,
    TaskItemStatus Status,
    DateOnly? DueDate
);
