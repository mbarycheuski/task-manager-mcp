namespace TaskManager.Mcp.Contracts.Inputs;

public record CreateTaskInput(
    string Title,
    string? Notes,
    TaskPriority? Priority,
    DateOnly? DueDate
);
