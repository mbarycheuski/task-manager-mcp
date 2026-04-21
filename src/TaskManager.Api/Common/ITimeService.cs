namespace TaskManager.Api.Common;

public interface ITimeService
{
    DateOnly GetTodayInDefaultTimezone();
}
