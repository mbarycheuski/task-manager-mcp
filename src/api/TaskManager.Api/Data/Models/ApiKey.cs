namespace TaskManager.Api.Data.Models;

public class ApiKey
{
    public Guid Id { get; set; }
    public string ClientName { get; set; } = null!;
    public string KeyHash { get; set; } = null!;
    public string Salt { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}
