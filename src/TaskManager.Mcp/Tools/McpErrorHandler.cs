using System.Net;
using ModelContextProtocol;

namespace TaskManager.Mcp.Tools;

internal static class McpErrorHandler
{
    internal static async Task<T> ExecuteAsync<T>(Func<Task<T>> func)
    {
        ArgumentNullException.ThrowIfNull(func);

        try
        {
            return await func();
        }
        catch (HttpRequestException ex)
        {
            throw MapToMcpException(ex);
        }
    }

    internal static async Task ExecuteAsync(Func<Task> func)
    {
        ArgumentNullException.ThrowIfNull(func);

        try
        {
            await func();
        }
        catch (HttpRequestException ex)
        {
            throw MapToMcpException(ex);
        }
    }

    private static McpException MapToMcpException(HttpRequestException ex) =>
        ex.StatusCode switch
        {
            HttpStatusCode.NotFound => new McpException("Task not found.", ex),
            HttpStatusCode.BadRequest => new McpException(
                "Invalid request: check that all required fields are provided and values are within allowed ranges.",
                ex
            ),
            HttpStatusCode.Unauthorized => new McpException(
                "Unauthorized: the API key is missing or invalid.",
                ex
            ),
            _ => new McpException($"Unexpected API error ({(int?)ex.StatusCode}).", ex),
        };
}
