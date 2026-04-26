namespace TaskManager.Mcp.Exceptions;

public sealed class NotFoundException(string message) : AppException(message);
