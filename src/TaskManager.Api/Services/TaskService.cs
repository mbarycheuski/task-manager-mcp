using TaskManager.Api.Contracts;
using TaskManager.Api.Contracts.Enums;
using TaskManager.Api.Exceptions;
using TaskManager.Api.Mappers;
using TaskManager.Api.Repositories;

namespace TaskManager.Api.Services;

public class TaskService(ITaskRepository repository, TimeProvider timeProvider) : ITaskService
{
    public async Task<IReadOnlyList<TaskItem>> GetAllAsync(
        TaskQueryFilters taskQueryFilters,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(taskQueryFilters);

        var tasks = await repository.GetAllAsync(
            taskQueryFilters.Statuses.ToDomain(),
            taskQueryFilters.Priority.ToDomain(),
            taskQueryFilters.DueDateFrom,
            taskQueryFilters.DueDateTo,
            cancellationToken
        );

        return [.. tasks.Select(t => t.ToContract())];
    }

    public async Task<TaskItem> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty);

        var task =
            await repository.GetByIdReadOnlyAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Task {id} not found.");

        return task.ToContract();
    }

    public async Task<TaskItem> CreateAsync(
        CreateTaskRequest createTaskRequest,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(createTaskRequest);

        var entity = createTaskRequest.ToDomain();

        await repository.AddAsync(entity, cancellationToken);

        return entity.ToContract();
    }

    public async Task<TaskItem> UpdateAsync(
        Guid id,
        UpdateTaskRequest updateTaskRequest,
        CancellationToken cancellationToken
    )
    {
        ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty);
        ArgumentNullException.ThrowIfNull(updateTaskRequest);

        var entity =
            await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Task {id} not found.");

        var currentStatus = entity.Status.ToContract();

        if (currentStatus == TaskItemStatus.Completed)
        {
            throw new BusinessException("Cannot modify a completed task.");
        }

        if (
            updateTaskRequest.Status == TaskItemStatus.Completed
            && currentStatus != TaskItemStatus.InProgress
        )
            throw new BusinessException(
                $"Invalid status transition from {currentStatus} to {updateTaskRequest.Status}."
            );

        var utcNow = timeProvider.GetUtcNow().UtcDateTime;

        entity.Title = updateTaskRequest.Title;
        entity.Notes = updateTaskRequest.Notes;
        entity.Priority = updateTaskRequest.Priority.ToDomain();
        entity.DueDate = updateTaskRequest.DueDate;
        entity.Status = updateTaskRequest.Status.ToDomain();
        entity.UpdatedAt = utcNow;

        if (updateTaskRequest.Status == TaskItemStatus.Completed)
            entity.CompletedAt = utcNow;

        await repository.UpdateAsync(entity, cancellationToken);

        return entity.ToContract();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty);

        var entity =
            await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Task {id} not found.");

        await repository.DeleteAsync(entity, cancellationToken);
    }
}
