using TaskManager.Api.OpenApi;

namespace TaskManager.Api.Contracts.Enums;

[SwaggerSchemaName("TaskStatus")]
public enum TaskItemStatus
{
    None = 0,
    InProgress,
    Completed,
}
