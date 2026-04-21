namespace TaskManager.Api.Exceptions;

public sealed class BusinessException(string message) : ApiException(message);
