using TaskManager.Mcp.Contracts.Inputs;
using TaskItemOutput = TaskManager.Mcp.Contracts.Outputs.TaskItem;
using TaskItemStatusOutput = TaskManager.Mcp.Contracts.TaskItemStatus;

namespace TaskManager.Mcp.Services;

public interface ITaskService
{
    Task<IReadOnlyList<TaskItemOutput>> GetAllAsync(
        IReadOnlyList<TaskItemStatusOutput>? statuses,
        DateOnly? dueDateFrom,
        DateOnly? dueDateTo,
        CancellationToken cancellationToken
    );

    Task<IReadOnlyList<TaskItemOutput>> GetCompletedAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<TaskItemOutput>> GetInProgressAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<TaskItemOutput>> GetOpenAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<TaskItemOutput>> GetTodayAsync(CancellationToken cancellationToken);

    Task<TaskItemOutput> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<TaskItemOutput> CreateAsync(CreateTaskInput input, CancellationToken cancellationToken);

    Task<TaskItemOutput> UpdateAsync(
        Guid id,
        UpdateTaskInput input,
        CancellationToken cancellationToken
    );

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
