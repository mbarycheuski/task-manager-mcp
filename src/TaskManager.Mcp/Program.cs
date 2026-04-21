using Microsoft.Extensions.Options;
using TaskManager.Mcp.Auth;
using TaskManager.Mcp.Collaborators;
using TaskManager.Mcp.Settings;

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
        httpClient.DefaultRequestHeaders.Add(ApiKeyDefaults.HeaderName, settings.ApiKey);
    }
);

builder.Services.AddMcpServer().WithHttpTransport(options => options.Stateless = true);

builder.Services.AddProblemDetails();

var app = builder.Build();

app.MapMcp();
app.Run();
