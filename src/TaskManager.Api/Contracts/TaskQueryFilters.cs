using TaskManager.Api.Contracts.Enums;

namespace TaskManager.Api.Contracts;

public record TaskQueryFilters(
    TaskItemStatus? Status,
    TaskPriority? Priority,
    DateOnly? DueDateFrom,
    DateOnly? DueDateTo
);
