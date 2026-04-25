using TaskManager.Mcp.Outputs;

namespace TaskManager.Mcp.Inputs;

public record CreateTaskInput(
    string Title,
    string? Notes,
    TaskPriority? Priority,
    DateOnly? DueDate
);
