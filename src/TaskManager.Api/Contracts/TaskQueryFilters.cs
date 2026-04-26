using TaskManager.Api.Contracts.Enums;

namespace TaskManager.Api.Contracts;

public record TaskQueryFilters(
    IReadOnlyList<TaskItemStatus>? Statuses,
    TaskPriority? Priority,
    DateOnly? DueDateFrom,
    DateOnly? DueDateTo
);
