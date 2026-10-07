namespace SalesHub.Domain.Entities;

public class Conversation
{
    public int ConversationId { get; set; }
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public int? ContactId { get; set; }
    public Contact? Contact { get; set; }
    public int? AssignedUserId { get; set; }
    public User? AssignedUser { get; set; }
    public string Channel { get; set; } = "WhatsApp";
    public string Status { get; set; } = "Open";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Message> Messages { get; set; } = new();
}
