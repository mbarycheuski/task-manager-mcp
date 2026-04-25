using System.Text.Json.Serialization;

namespace TaskManager.Mcp.Collaborators.Dto;

public record UpdateTaskRequestDto(
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("notes")] string? Notes,
    [property: JsonPropertyName("priority")] TaskPriorityDto? Priority,
    [property: JsonPropertyName("status")] TaskItemStatusDto Status,
    [property: JsonPropertyName("dueDate")] DateOnly? DueDate
);
