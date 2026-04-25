using System.Text.Json.Serialization;

namespace TaskManager.Mcp.Outputs;

public record TaskItem(
    Guid Id,
    string Title,
    TaskItemStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? Notes = null,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        TaskPriority? Priority = null,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        DateOnly? DueDate = null,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        DateTime? CompletedAt = null
);
