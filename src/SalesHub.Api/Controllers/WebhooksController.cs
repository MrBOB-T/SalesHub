using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesHub.Application.Conversations;
using SalesHub.Domain.Entities;
using SalesHub.Infrastructure.Persistence;

namespace SalesHub.Api.Controllers;

[ApiController]
[Route("api/webhooks/whatsapp")]
public class WebhooksController(SalesHubDbContext db) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Inbound(InboundMessageRequest r, CancellationToken ct)
    {
        // Idempotency: the same webhook delivered twice must not create two messages
        if (!string.IsNullOrWhiteSpace(r.ExternalMessageId) &&
            await db.Messages.AnyAsync(m => m.ExternalMessageId == r.ExternalMessageId, ct))
            return Ok(new { status = "Duplicate ignored" });

        // Identify customer by contact phone, then customer phone
        var contact = await db.Contacts.FirstOrDefaultAsync(c => c.Phone == r.FromPhone, ct);
        var customer = contact is not null
            ? await db.Customers.FindAsync([contact.CustomerId], ct)
            : await db.Customers.FirstOrDefaultAsync(c => c.Phone == r.FromPhone, ct);

        if (customer is null)
            return UnprocessableEntity($"Unknown sender {r.FromPhone}. Add the number to a customer or contact first.");

        // Reuse the open WhatsApp conversation or start a new one
        var conv = await db.Conversations.FirstOrDefaultAsync(c =>
            c.CustomerId == customer.CustomerId && c.Channel == "WhatsApp" && c.Status == "Open", ct);
        if (conv is null)
        {
            conv = new Conversation
            {
                CustomerId = customer.CustomerId,
                ContactId = contact?.ContactId,
                AssignedUserId = customer.AssignedUserId
            };
            db.Conversations.Add(conv);
        }

        conv.Messages.Add(new Message
        {
            Direction = "Inbound",
            SenderPhone = r.FromPhone,
            MessageText = r.Text,
            Status = "Received",
            ExternalMessageId = string.IsNullOrWhiteSpace(r.ExternalMessageId)
                ? $"mock-in-{Guid.NewGuid():N}" : r.ExternalMessageId
        });

        // Follow-up task for the assigned salesperson (Elsa will take this over later)
        db.Activities.Add(new Activity
        {
            CustomerId = customer.CustomerId,
            UserId = customer.AssignedUserId,
            Type = "Reply",
            Description = $"New WhatsApp message from {contact?.Name ?? r.FromPhone} ({customer.CompanyName})",
            DueDate = DateTime.UtcNow.AddHours(4)
        });

        db.AuditLogs.Add(new AuditLog { Entity = "Message", EntityId = customer.CustomerId.ToString(), Action = "Received" });
        await db.SaveChangesAsync(ct);

        return Ok(new { status = "Stored", conversationId = conv.ConversationId });
    }
}