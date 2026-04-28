using System.Reflection;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using TaskManager.Api.Auth;
using TaskManager.Api.Common;
using TaskManager.Api.Data;
using TaskManager.Api.Exceptions.Handlers;
using TaskManager.Api.OpenApi;
using TaskManager.Api.Repositories;
using TaskManager.Api.Services;
using TaskManager.Api.Settings;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
    );

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type =>
        type.GetCustomAttribute<SwaggerSchemaNameAttribute>()?.Name ?? type.Name
    );
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
    options.SchemaFilter<StringEnumSchemaFilter>();
    options.AddSecurityDefinition(
        ApiKeyDefaults.AuthenticationScheme,
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Name = ApiKeyDefaults.HeaderName,
            Type = SecuritySchemeType.ApiKey,
            Description = "API key authentication via X-Api-Key header",
        }
    );
    options.AddSecurityRequirement(doc =>
    {
        var requirement = new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(ApiKeyDefaults.AuthenticationScheme, doc)] = [],
        };

        return requirement;
    });
});

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IApiKeyHasher, ApiKeyHasher>();
builder.Services.AddSingleton<IApiKeyCacheService, ApiKeyCacheService>();
builder.Services.AddScoped<ApiKeySeeder>();
builder
    .Services.AddAuthentication(ApiKeyDefaults.AuthenticationScheme)
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(
        ApiKeyDefaults.AuthenticationScheme,
        null
    );

builder.Services.AddSingleton(TimeProvider.System);
builder
    .Services.AddOptions<AppSettings>()
    .BindConfiguration(AppSettings.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddSingleton<ITimeService, TimeService>();

builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
    db.Database.Migrate();
    await scope
        .ServiceProvider.GetRequiredService<ApiKeySeeder>()
        .SeedAsync(app.Lifetime.ApplicationStopping);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.RoutePrefix = "docs";
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskManager API v1");
    });
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
