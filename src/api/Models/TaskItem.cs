using TaskManager.Api.Models.Enums;

namespace TaskManager.Api.Models;

public class TaskItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public TaskPriority? Priority { get; set; }
    public TaskItemStatus Status { get; set; } = TaskItemStatus.None;
    public DateOnly? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
