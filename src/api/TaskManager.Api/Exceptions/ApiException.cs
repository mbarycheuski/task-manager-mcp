namespace TaskManager.Api.Exceptions;

public abstract class ApiException(string message) : Exception(message);
