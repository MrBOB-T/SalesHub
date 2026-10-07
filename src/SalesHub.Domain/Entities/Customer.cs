namespace SalesHub.Domain.Entities;

public class Customer
{
    public int CustomerId { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public int? AssignedUserId { get; set; }
    public User? AssignedUser { get; set; }
    public string Status { get; set; } = "Active";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Contact> Contacts { get; set; } = new();
    public List<Conversation> Conversations { get; set; } = new();
    public List<Activity> Activities { get; set; } = new();
}
