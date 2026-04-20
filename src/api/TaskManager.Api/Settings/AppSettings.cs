using System.ComponentModel.DataAnnotations;

namespace TaskManager.Api.Settings;

public class AppSettings
{
    public const string SectionName = "AppSettings";

    [Required]
    public required string Timezone { get; init; }
}
