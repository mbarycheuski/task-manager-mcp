using Microsoft.Extensions.Options;
using ModelContextProtocol.Protocol;
using TaskManager.Mcp.Collaborators;
using TaskManager.Mcp.Common;
using TaskManager.Mcp.Exceptions;
using TaskManager.Mcp.Prompts;
using TaskManager.Mcp.Providers;
using TaskManager.Mcp.Resources;
using TaskManager.Mcp.Services;
using TaskManager.Mcp.Settings;
using TaskManager.Mcp.Tools;
using TaskManager.Mcp.Utilities.Serializers;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services.AddOptions<McpSettings>()
    .BindConfiguration(McpSettings.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddHttpClient<ITaskApiCollaborator, TaskApiCollaborator>(
    (serviceProvider, httpClient) =>
    {
        var settings = serviceProvider.GetRequiredService<IOptions<McpSettings>>().Value;
        httpClient.BaseAddress = new Uri(settings.ApiBaseUrl);
        httpClient.DefaultRequestHeaders.Add(TaskApiConstants.Headers.ApiKey, settings.ApiKey);
    }
);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<ITimeService, TimeService>();
builder.Services.AddSingleton<IOutputSerializer, JsonOutputSerializer>();
builder.Services.AddSingleton<IPromptProvider, EmbeddedResourcePromptProvider>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IPromptService, PromptService>();
builder.Services.AddScoped<TaskTools>();
builder.Services.AddScoped<TaskResources>();
builder.Services.AddScoped<TaskPrompts>();

builder
    .Services.AddMcpServer()
    .WithHttpTransport(options => options.Stateless = true)
    .WithTools<TaskTools>()
    .WithResources<TaskResources>()
    .WithPrompts<TaskPrompts>()
    .WithRequestFilters(requestFilters =>
    {
        requestFilters.AddCallToolFilter(next =>
            async (context, cancellationToken) =>
            {
                try
                {
                    return await next(context, cancellationToken);
                }
                catch (AppException ex)
                {
                    return new CallToolResult
                    {
                        Content = [new TextContentBlock { Text = ex.Message }],
                        IsError = true,
                    };
                }
            }
        );
    });

builder.Services.AddProblemDetails();

var app = builder.Build();

app.MapMcp();
app.Run();
