using TaskManager.Mcp.Collaborators;
using TaskManager.Mcp.Collaborators.Dto;
using TaskManager.Mcp.Common;
using TaskManager.Mcp.Inputs;
using TaskManager.Mcp.Mappers;
using TaskItemOutput = TaskManager.Mcp.Outputs.TaskItem;

namespace TaskManager.Mcp.Services;

public class TaskService(ITaskApiCollaborator collaborator, ITimeService timeService) : ITaskService
{
    public async Task<IReadOnlyList<TaskItemOutput>> GetAllAsync(
        CancellationToken cancellationToken
    )
    {
        var dtos = await collaborator.GetAllAsync(null, null, null, cancellationToken);

        return [.. dtos.Select(dto => dto.ToOutput())];
    }

    public async Task<IReadOnlyList<TaskItemOutput>> GetCompletedAsync(
        CancellationToken cancellationToken
    )
    {
        var dtos = await collaborator.GetAllAsync(
            TaskItemStatusDto.Completed,
            null,
            null,
            cancellationToken
        );

        return [.. dtos.Select(dto => dto.ToOutput())];
    }

    public async Task<IReadOnlyList<TaskItemOutput>> GetInProgressAsync(
        CancellationToken cancellationToken
    )
    {
        var dtos = await collaborator.GetAllAsync(
            TaskItemStatusDto.InProgress,
            null,
            null,
            cancellationToken
        );

        return [.. dtos.Select(dto => dto.ToOutput())];
    }

    public async Task<IReadOnlyList<TaskItemOutput>> GetTodayAsync(
        CancellationToken cancellationToken
    )
    {
        var today = timeService.GetTodayInDefaultTimezone();

        var dtos = await collaborator.GetAllAsync(null, today, today, cancellationToken);

        return [.. dtos.Select(dto => dto.ToOutput())];
    }

    public async Task<TaskItemOutput> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty);

        var dto = await collaborator.GetByIdAsync(id, cancellationToken);

        return dto.ToOutput();
    }

    public async Task<TaskItemOutput> CreateAsync(
        CreateTaskInput input,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(input);

        var dto = await collaborator.CreateAsync(input.ToDto(), cancellationToken);

        return dto.ToOutput();
    }

    public async Task<TaskItemOutput> UpdateAsync(
        Guid id,
        UpdateTaskInput input,
        CancellationToken cancellationToken
    )
    {
        ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty);
        ArgumentNullException.ThrowIfNull(input);

        var dto = await collaborator.UpdateAsync(id, input.ToDto(), cancellationToken);

        return dto.ToOutput();
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty);

        return collaborator.DeleteAsync(id, cancellationToken);
    }
}
