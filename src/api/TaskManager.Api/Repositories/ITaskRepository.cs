using TaskManager.Api.Data.Models;
using TaskManager.Api.Data.Models.Enums;

namespace TaskManager.Api.Repositories;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<TaskItem?> GetByIdReadOnlyAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<TaskItem>> GetAllAsync(
        TaskItemStatus? status,
        TaskPriority? priority,
        DateOnly? dueDateFrom,
        DateOnly? dueDateTo,
        CancellationToken cancellationToken
    );
    Task AddAsync(TaskItem taskItem, CancellationToken cancellationToken);
    Task UpdateAsync(TaskItem taskItem, CancellationToken cancellationToken);
    Task DeleteAsync(TaskItem taskItem, CancellationToken cancellationToken);
}
