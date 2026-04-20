using TaskManager.Api.Contracts.Enums;
using TaskManager.Api.OpenApi;

namespace TaskManager.Api.Contracts;

[SwaggerSchemaName("Task")]
public record TaskItem(
    Guid Id,
    string Title,
    string? Notes,
    TaskPriority? Priority,
    TaskItemStatus Status,
    DateOnly? DueDate,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime? CompletedAt
);
