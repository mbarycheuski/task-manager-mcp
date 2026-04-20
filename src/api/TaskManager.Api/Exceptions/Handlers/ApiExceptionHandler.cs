using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TaskManager.Api.Exceptions.Handlers;

public sealed class ApiExceptionHandler(
    ILogger<ApiExceptionHandler> logger,
    IProblemDetailsService problemDetailsService,
    IHostEnvironment hostEnvironment
) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        var (statusCode, title, type) = MapException(exception);

        logger.Log(
            LogLevel.Error,
            exception,
            "Exception {ExceptionType} returning {StatusCode}. TraceId: {TraceId}",
            exception.GetType().Name,
            statusCode,
            httpContext.TraceIdentifier
        );

        httpContext.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Type = type,
            Instance = httpContext.Request.Path,
            Detail = GetSafeErrorMessage(exception),
            Extensions = { ["traceId"] = httpContext.TraceIdentifier },
        };

        return await problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext { HttpContext = httpContext, ProblemDetails = problemDetails }
        );
    }

    private static (int StatusCode, string Title, string Type) MapException(Exception exception) =>
        exception switch
        {
            NotFoundException => (
                StatusCodes.Status404NotFound,
                "Resource not found",
                "https://tools.ietf.org/html/rfc9110#section-15.5.5"
            ),
            BusinessException => (
                StatusCodes.Status400BadRequest,
                "Business rule violation",
                "https://tools.ietf.org/html/rfc9110#section-15.5.1"
            ),
            ArgumentNullException or ArgumentException or ArgumentOutOfRangeException => (
                StatusCodes.Status400BadRequest,
                "Invalid argument provided",
                "https://tools.ietf.org/html/rfc9110#section-15.5.1"
            ),
            _ => (
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred",
                "https://tools.ietf.org/html/rfc9110#section-15.6.1"
            ),
        };

    private string? GetSafeErrorMessage(Exception exception)
    {
        if (hostEnvironment.IsDevelopment())
        {
            return exception.Message;
        }

        return exception is ApiException ? exception.Message : null;
    }
}
