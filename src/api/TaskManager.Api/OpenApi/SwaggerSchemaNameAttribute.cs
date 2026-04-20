namespace TaskManager.Api.OpenApi;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Struct)]
public sealed class SwaggerSchemaNameAttribute(string name) : Attribute
{
    public string Name { get; } =
        string.IsNullOrWhiteSpace(name)
            ? throw new ArgumentException("Name must not be empty.", nameof(name))
            : name;
}
