namespace TaskManager.Mcp.Collaborators;

public static class TaskApiConstants
{
    public static class Endpoints
    {
        public const string GetTasks = "/api/tasks";
        public const string GetTaskById = "/api/tasks/{0}";
        public const string CreateTask = "/api/tasks";
        public const string UpdateTask = "/api/tasks/{0}";
        public const string DeleteTask = "/api/tasks/{0}";
    }

    public static class Headers
    {
        public const string ApiKey = "X-Api-Key";
    }
}
