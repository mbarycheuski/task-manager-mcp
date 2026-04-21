using Microsoft.Extensions.Options;
using TaskManager.Api.Settings;

namespace TaskManager.Api.Common;

public class TimeService(TimeProvider timeProvider, IOptions<AppSettings> appSettings)
    : ITimeService
{
    public DateOnly GetTodayInDefaultTimezone()
    {
        var timezone = TimeZoneInfo.FindSystemTimeZoneById(appSettings.Value.Timezone);

        var localNow = TimeZoneInfo.ConvertTimeFromUtc(
            timeProvider.GetUtcNow().UtcDateTime,
            timezone
        );

        return DateOnly.FromDateTime(localNow);
    }
}
