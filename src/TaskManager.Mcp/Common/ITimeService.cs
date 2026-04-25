namespace TaskManager.Mcp.Common;

public interface ITimeService
{
    DateOnly GetTodayInDefaultTimezone();
}
