using TaskManager.Api.Contracts;

namespace TaskManager.Api.Services;

public interface ITaskService
{
    Task<IReadOnlyList<TaskItem>> GetAllAsync(
        TaskQueryFilters taskQueryFilters,
        CancellationToken cancellationToken
    );
    Task<TaskItem> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<TaskItem> CreateAsync(
        CreateTaskRequest createTaskRequest,
        CancellationToken cancellationToken
    );
    Task<TaskItem> UpdateAsync(
        Guid id,
        UpdateTaskRequest updateTaskRequest,
        CancellationToken cancellationToken
    );
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
