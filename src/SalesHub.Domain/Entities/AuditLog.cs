namespace SalesHub.Domain.Entities;

public class AuditLog
{
    public int AuditId { get; set; }
    public int? UserId { get; set; }
    public string Entity { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Details { get; set; }
}
