namespace TaskManager.Mcp.Utilities.Serializers;

public interface IOutputSerializer
{
    string Serialize<T>(T value);
}
