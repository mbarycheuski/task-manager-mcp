using System.ComponentModel.DataAnnotations;

namespace TaskManager.Mcp.Settings;

public class McpSettings
{
    public const string SectionName = "AppSettings";

    [Required]
    public required string ApiBaseUrl { get; init; }

    [Required]
    public required string ApiKey { get; init; }

    [Required]
    public required string Timezone { get; init; }
}
