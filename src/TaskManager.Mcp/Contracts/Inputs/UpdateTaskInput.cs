namespace TaskManager.Mcp.Contracts.Inputs;

public record UpdateTaskInput(
    string Title,
    string? Notes,
    TaskPriority? Priority,
    TaskItemStatus Status,
    DateOnly? DueDate
);
