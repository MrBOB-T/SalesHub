namespace SalesHub.Domain.Entities;

public class Message
{
    public int MessageId { get; set; }
    public int ConversationId { get; set; }
    public Conversation? Conversation { get; set; }
    public string Direction { get; set; } = "Inbound"; // Inbound / Outbound
    public string? SenderPhone { get; set; }
    public string MessageText { get; set; } = string.Empty;
    public string Status { get; set; } = "Received";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? ExternalMessageId { get; set; } // used for idempotency later
}
