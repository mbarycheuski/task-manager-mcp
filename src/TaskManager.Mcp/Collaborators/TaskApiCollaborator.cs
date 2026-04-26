using System.Text.Json;
using System.Text.Json.Serialization;
using TaskManager.Mcp.Collaborators.Dto;
using TaskManager.Mcp.Common;

namespace TaskManager.Mcp.Collaborators;

public class TaskApiCollaborator(HttpClient httpClient) : ITaskApiCollaborator
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
    };

    public async Task<IReadOnlyList<TaskItemDto>> GetAllAsync(
        IReadOnlyList<TaskItemStatusDto>? statuses,
        DateOnly? dueDateFrom,
        DateOnly? dueDateTo,
        CancellationToken cancellationToken
    )
    {
        var queryParts = new List<string>();
        if (statuses is not null)
            foreach (var s in statuses)
                queryParts.Add($"statuses={s}");
        if (dueDateFrom.HasValue)
            queryParts.Add($"dueDateFrom={dueDateFrom.Value.ToString(DateFormats.Default)}");
        if (dueDateTo.HasValue)
            queryParts.Add($"dueDateTo={dueDateTo.Value.ToString(DateFormats.Default)}");

        var endpoint =
            queryParts.Count > 0
                ? $"{TaskApiConstants.Endpoints.GetTasks}?{string.Join("&", queryParts)}"
                : TaskApiConstants.Endpoints.GetTasks;

        var response = await httpClient.GetAsync(endpoint, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<TaskItemDto>>(
                _jsonSerializerOptions,
                cancellationToken
            ) ?? [];
    }

    public async Task<TaskItemDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty);

        var endpoint = string.Format(TaskApiConstants.Endpoints.GetTaskById, id);
        var response = await httpClient.GetAsync(endpoint, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TaskItemDto>(
                _jsonSerializerOptions,
                cancellationToken
            ) ?? throw new InvalidOperationException($"Failed to deserialize task {id}.");
    }

    public async Task<TaskItemDto> CreateAsync(
        CreateTaskRequestDto request,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(request);

        var response = await httpClient.PostAsJsonAsync(
            TaskApiConstants.Endpoints.CreateTask,
            request,
            _jsonSerializerOptions,
            cancellationToken
        );

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TaskItemDto>(
                _jsonSerializerOptions,
                cancellationToken
            ) ?? throw new InvalidOperationException("Failed to deserialize created task.");
    }

    public async Task<TaskItemDto> UpdateAsync(
        Guid id,
        UpdateTaskRequestDto request,
        CancellationToken cancellationToken
    )
    {
        ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty);
        ArgumentNullException.ThrowIfNull(request);

        var endpoint = string.Format(TaskApiConstants.Endpoints.UpdateTask, id);
        var response = await httpClient.PutAsJsonAsync(
            endpoint,
            request,
            _jsonSerializerOptions,
            cancellationToken
        );

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TaskItemDto>(
                _jsonSerializerOptions,
                cancellationToken
            ) ?? throw new InvalidOperationException($"Failed to deserialize updated task {id}.");
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(id, Guid.Empty);

        var endpoint = string.Format(TaskApiConstants.Endpoints.DeleteTask, id);
        var response = await httpClient.DeleteAsync(endpoint, cancellationToken);

        response.EnsureSuccessStatusCode();
    }
}
