namespace TaskManager.Mcp.Common.Services;

public interface ITimeService
{
    DateOnly GetTodayInDefaultTimezone();
}
