using TaskManager.Mcp.Collaborators;
using TaskManager.Mcp.Collaborators.Dto;
using TaskManager.Mcp.Common;
using TaskManager.Mcp.Common.Services;
using TaskManager.Mcp.Exceptions;
using TaskManager.Mcp.Inputs;
using TaskManager.Mcp.Mappers;
using TaskItemOutput = TaskManager.Mcp.Outputs.TaskItem;
using TaskItemStatusOutput = TaskManager.Mcp.Outputs.TaskItemStatus;

namespace TaskManager.Mcp.Services;

public class TaskService(ITaskApiCollaborator collaborator, ITimeService timeService) : ITaskService
{
    public Task<IReadOnlyList<TaskItemOutput>> GetAllAsync(
        IReadOnlyList<TaskItemStatusOutput>? statuses,
        DateOnly? dueDateFrom,
        DateOnly? dueDateTo,
        CancellationToken cancellationToken
    )
    {
        if (dueDateFrom.HasValue && dueDateTo.HasValue && dueDateFrom.Value > dueDateTo.Value)
        {
            throw new ValidationException("Due date range start cannot be later than its end.");
        }

        return ApiErrorHandler.ExecuteAsync<IReadOnlyList<TaskItemOutput>>(async () =>
        {
            var dtos = await collaborator.GetAllAsync(
                statuses.ToDto(),
                dueDateFrom,
                dueDateTo,
                cancellationToken
            );

            return [.. dtos.Select(dto => dto.ToOutput())];
        });
    }

    public Task<IReadOnlyList<TaskItemOutput>> GetCompletedAsync(
        CancellationToken cancellationToken
    )
    {
        return ApiErrorHandler.ExecuteAsync<IReadOnlyList<TaskItemOutput>>(async () =>
        {
            var dtos = await collaborator.GetAllAsync(
                [TaskItemStatusDto.Completed],
                null,
                null,
                cancellationToken
            );

            return [.. dtos.Select(dto => dto.ToOutput())];
        });
    }

    public Task<IReadOnlyList<TaskItemOutput>> GetInProgressAsync(
        CancellationToken cancellationToken
    )
    {
        return ApiErrorHandler.ExecuteAsync<IReadOnlyList<TaskItemOutput>>(async () =>
        {
            var dtos = await collaborator.GetAllAsync(
                [TaskItemStatusDto.InProgress],
                null,
                null,
                cancellationToken
            );

            return [.. dtos.Select(dto => dto.ToOutput())];
        });
    }

    public Task<IReadOnlyList<TaskItemOutput>> GetOpenAsync(CancellationToken cancellationToken)
    {
        return ApiErrorHandler.ExecuteAsync<IReadOnlyList<TaskItemOutput>>(async () =>
        {
            var dtos = await collaborator.GetAllAsync(
                [TaskItemStatusDto.None, TaskItemStatusDto.InProgress],
                null,
                null,
                cancellationToken
            );

            return [.. dtos.Select(dto => dto.ToOutput())];
        });
    }

    public Task<IReadOnlyList<TaskItemOutput>> GetTodayAsync(CancellationToken cancellationToken)
    {
        return ApiErrorHandler.ExecuteAsync<IReadOnlyList<TaskItemOutput>>(async () =>
        {
            var today = timeService.GetTodayInDefaultTimezone();
            var dtos = await collaborator.GetAllAsync(null, today, today, cancellationToken);

            return [.. dtos.Select(dto => dto.ToOutput())];
        });
    }

    public Task<TaskItemOutput> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty);

        return ApiErrorHandler.ExecuteAsync(async () =>
        {
            var dto = await collaborator.GetByIdAsync(id, cancellationToken);

            return dto.ToOutput();
        });
    }

    public Task<TaskItemOutput> CreateAsync(
        CreateTaskInput input,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(input);

        if (input.DueDate.HasValue && input.DueDate < timeService.GetTodayInDefaultTimezone())
        {
            throw new ValidationException("Due date cannot be in the past.");
        }

        return ApiErrorHandler.ExecuteAsync<TaskItemOutput>(async () =>
        {
            var dto = await collaborator.CreateAsync(input.ToDto(), cancellationToken);

            return dto.ToOutput();
        });
    }

    public Task<TaskItemOutput> UpdateAsync(
        Guid id,
        UpdateTaskInput input,
        CancellationToken cancellationToken
    )
    {
        ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty);
        ArgumentNullException.ThrowIfNull(input);

        if (input.DueDate.HasValue && input.DueDate < timeService.GetTodayInDefaultTimezone())
        {
            throw new ValidationException("Due date cannot be in the past.");
        }

        return ApiErrorHandler.ExecuteAsync(async () =>
        {
            var dto = await collaborator.UpdateAsync(id, input.ToDto(), cancellationToken);

            return dto.ToOutput();
        });
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty);

        return ApiErrorHandler.ExecuteAsync(() => collaborator.DeleteAsync(id, cancellationToken));
    }
}
