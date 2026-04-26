namespace TaskManager.Mcp.Providers;

public interface IPromptProvider
{
    Task<string> GetAsync(string resourceName, CancellationToken cancellationToken);
}
