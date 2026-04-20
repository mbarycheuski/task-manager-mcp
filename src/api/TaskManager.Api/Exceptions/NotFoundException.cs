namespace TaskManager.Api.Exceptions;

public sealed class NotFoundException(string message) : ApiException(message);
