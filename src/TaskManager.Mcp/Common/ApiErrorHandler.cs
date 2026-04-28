using System.Net;
using TaskManager.Mcp.Exceptions;

namespace TaskManager.Mcp.Common;

public static class ApiErrorHandler
{
    public static async Task<T> ExecuteAsync<T>(Func<Task<T>> func)
    {
        ArgumentNullException.ThrowIfNull(func);

        try
        {
            return await func();
        }
        catch (HttpRequestException ex)
        {
            throw HandleHttpError(ex);
        }
    }

    public static async Task ExecuteAsync(Func<Task> func)
    {
        ArgumentNullException.ThrowIfNull(func);

        try
        {
            await func();
        }
        catch (HttpRequestException ex)
        {
            throw HandleHttpError(ex);
        }
    }

    private static AppException HandleHttpError(HttpRequestException ex) =>
        ex.StatusCode switch
        {
            HttpStatusCode.NotFound => new NotFoundException("Resource not found."),
            HttpStatusCode.BadRequest => new AppException(
                "Invalid request: check that all required fields are provided and values are within allowed ranges."
            ),
            HttpStatusCode.Unauthorized => new AppException(
                "Unauthorized: the API key is missing or invalid."
            ),
            _ => new AppException($"Unexpected API error ({(int?)ex.StatusCode})."),
        };
}
