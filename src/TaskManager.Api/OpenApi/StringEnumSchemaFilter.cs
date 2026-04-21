using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TaskManager.Api.OpenApi;

public class StringEnumSchemaFilter : ISchemaFilter
{
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema is not OpenApiSchema concreteSchema)
            return;

        var type = Nullable.GetUnderlyingType(context.Type) ?? context.Type;

        if (!type.IsEnum)
            return;

        concreteSchema.Type = JsonSchemaType.String;
        concreteSchema.Format = null;
        concreteSchema.Enum ??= [];
        concreteSchema.Enum.Clear();

        foreach (var name in Enum.GetNames(type))
            concreteSchema.Enum.Add(JsonValue.Create(name)!);
    }
}
