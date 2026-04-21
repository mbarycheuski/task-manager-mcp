using TaskManager.Api.Contracts.Enums;

namespace TaskManager.Api.Contracts;

public record CreateTaskRequest(
    string Title,
    string? Notes,
    TaskPriority? Priority,
    DateOnly? DueDate
);
