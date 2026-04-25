using TaskManager.Mcp.Outputs;

namespace TaskManager.Mcp.Inputs;

public record UpdateTaskInput(
    string Title,
    string? Notes,
    TaskPriority? Priority,
    TaskItemStatus Status,
    DateOnly? DueDate
);
