namespace SalesHub.Domain.Entities;

public class Activity
{
    public int ActivityId { get; set; }
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    public string Type { get; set; } = "FollowUp";
    public string Description { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedDate { get; set; }
}
