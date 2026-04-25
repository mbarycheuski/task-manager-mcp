using System.Text.Json.Serialization;

namespace TaskManager.Mcp.Collaborators.Dto;

public record TaskItemDto(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("notes")] string? Notes,
    [property: JsonPropertyName("priority")] TaskPriorityDto? Priority,
    [property: JsonPropertyName("status")] TaskItemStatusDto Status,
    [property: JsonPropertyName("dueDate")] DateOnly? DueDate,
    [property: JsonPropertyName("createdAt")] DateTime CreatedAt,
    [property: JsonPropertyName("updatedAt")] DateTime UpdatedAt,
    [property: JsonPropertyName("completedAt")] DateTime? CompletedAt
);
