namespace TaskManager.Mcp.Common.Serializers;

public interface IOutputSerializer
{
    string Serialize<T>(T value);
}
