namespace TaskManager.Mcp.Exceptions;

public sealed class ValidationException(string message) : AppException(message);
