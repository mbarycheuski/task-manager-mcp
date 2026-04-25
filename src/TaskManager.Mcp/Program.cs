using Microsoft.Extensions.Options;
using TaskManager.Mcp.Collaborators;
using TaskManager.Mcp.Common;
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
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<TaskTools>();
builder.Services.AddScoped<TaskResources>();

builder
    .Services.AddMcpServer()
    .WithHttpTransport(options => options.Stateless = true)
    .WithTools<TaskTools>()
    .WithResources<TaskResources>();

builder.Services.AddProblemDetails();

var app = builder.Build();

app.MapMcp();
app.Run();
