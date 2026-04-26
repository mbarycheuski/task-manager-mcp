using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Data;
using TaskManager.Api.Data.Models;
using TaskManager.Api.Data.Models.Enums;

namespace TaskManager.Api.Repositories;

public class TaskRepository(TaskDbContext taskDbContext) : ITaskRepository
{
    public Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty);

        return taskDbContext.Tasks.SingleOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public Task<TaskItem?> GetByIdReadOnlyAsync(Guid id, CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty);

        return taskDbContext
            .Tasks.AsNoTracking()
            .SingleOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<TaskItem>> GetAllAsync(
        IReadOnlyList<TaskItemStatus>? statuses,
        TaskPriority? priority,
        DateOnly? dueDateFrom,
        DateOnly? dueDateTo,
        CancellationToken cancellationToken
    )
    {
        var query = taskDbContext.Tasks.AsNoTracking().AsQueryable();

        if (statuses?.Count > 0)
        {
            query = query.Where(t => statuses.Contains(t.Status));
        }

        if (priority.HasValue)
        {
            query = query.Where(t => t.Priority == priority.Value);
        }

        if (dueDateFrom.HasValue)
        {
            query = query.Where(t => t.DueDate >= dueDateFrom.Value);
        }

        if (dueDateTo.HasValue)
        {
            query = query.Where(t => t.DueDate <= dueDateTo.Value);
        }

        return await query.OrderByDescending(t => t.CreatedAt).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(TaskItem taskItem, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(taskItem);

        taskDbContext.Tasks.Add(taskItem);

        await taskDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(TaskItem taskItem, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(taskItem);

        await taskDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(TaskItem taskItem, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(taskItem);

        taskDbContext.Tasks.Remove(taskItem);

        await taskDbContext.SaveChangesAsync(cancellationToken);
    }
}
