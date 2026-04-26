using Microsoft.Extensions.Options;
using TaskManager.Mcp.Settings;

namespace TaskManager.Mcp.Common.Services;

public class TimeService(TimeProvider timeProvider, IOptions<McpSettings> mcpSettings)
    : ITimeService
{
    public DateOnly GetTodayInDefaultTimezone()
    {
        var timezone = TimeZoneInfo.FindSystemTimeZoneById(mcpSettings.Value.Timezone);

        var localNow = TimeZoneInfo.ConvertTimeFromUtc(
            timeProvider.GetUtcNow().UtcDateTime,
            timezone
        );

        return DateOnly.FromDateTime(localNow);
    }
}
