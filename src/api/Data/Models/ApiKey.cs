namespace TaskManager.Api.Data.Models;

public class ApiKey
{
    public Guid Id { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string KeyHash { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}
