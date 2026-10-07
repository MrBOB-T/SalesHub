using System.ComponentModel.DataAnnotations;

namespace SalesHub.Application.Conversations;

public record ConversationSummaryDto(int ConversationId, int CustomerId, string CompanyName,
    string? ContactName, int? AssignedUserId, string Status,
    string? LastMessage, DateTime? LastMessageAt);

public record MessageDto(int MessageId, string Direction, string? SenderPhone,
    string MessageText, string Status, DateTime Timestamp);

public record ConversationDetailDto(int ConversationId, int CustomerId, string CompanyName,
    string? ContactName, string Status, List<MessageDto> Messages);

public record SendMessageRequest([Required, MaxLength(4096)] string Text);

public record InboundMessageRequest(
    [Required] string FromPhone,
    [Required, MaxLength(4096)] string Text,
    string? ExternalMessageId);