using System.Reflection;

namespace TaskManager.Mcp.Providers;

public class EmbeddedResourcePromptProvider : IPromptProvider
{
    private const string ResourcePath = "TaskManager.Mcp.Prompts.Content";
    private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();

    public Task<string> GetAsync(string resourceName, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceName);

        var fullResourceName = $"{ResourcePath}.{resourceName}";
        using var stream =
            _assembly.GetManifestResourceStream(fullResourceName)
            ?? throw new InvalidOperationException(
                $"Embedded resource '{fullResourceName}' not found in assembly."
            );

        using var reader = new StreamReader(stream);

        return reader.ReadToEndAsync(cancellationToken);
    }
}
