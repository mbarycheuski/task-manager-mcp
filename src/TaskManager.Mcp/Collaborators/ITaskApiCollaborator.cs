using TaskManager.Mcp.Collaborators.Dto;

namespace TaskManager.Mcp.Collaborators;

public interface ITaskApiCollaborator
{
    Task<IReadOnlyList<TaskItemDto>> GetAllAsync(
        IReadOnlyList<TaskItemStatusDto>? statuses,
        DateOnly? dueDateFrom,
        DateOnly? dueDateTo,
        CancellationToken cancellationToken
    );

    Task<TaskItemDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<TaskItemDto> CreateAsync(
        CreateTaskRequestDto request,
        CancellationToken cancellationToken
    );

    Task<TaskItemDto> UpdateAsync(
        Guid id,
        UpdateTaskRequestDto request,
        CancellationToken cancellationToken
    );

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
