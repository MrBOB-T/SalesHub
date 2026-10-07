using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesHub.Application.Conversations;
using SalesHub.Application.Messaging;
using SalesHub.Domain.Entities;
using SalesHub.Infrastructure.Persistence;

namespace SalesHub.Api.Controllers;

[ApiController]
[Route("api/conversations")]
public class ConversationsController(SalesHubDbContext db, IWhatsAppService whatsApp) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<ConversationSummaryDto>> GetAll([FromQuery] int? customerId)
    {
        var q = db.Conversations.AsNoTracking();
        if (customerId is not null) q = q.Where(c => c.CustomerId == customerId);

        return await q
            .Select(c => new ConversationSummaryDto(
                c.ConversationId, c.CustomerId, c.Customer!.CompanyName,
                c.Contact != null ? c.Contact.Name : null,
                c.AssignedUserId, c.Status,
                c.Messages.OrderByDescending(m => m.Timestamp).Select(m => m.MessageText).FirstOrDefault(),
                c.Messages.OrderByDescending(m => m.Timestamp).Select(m => (DateTime?)m.Timestamp).FirstOrDefault()))
            .OrderByDescending(c => c.LastMessageAt)
            .ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ConversationDetailDto>> Get(int id)
    {
        var c = await db.Conversations.AsNoTracking()
            .Include(x => x.Customer).Include(x => x.Contact)
            .Include(x => x.Messages)
            .FirstOrDefaultAsync(x => x.ConversationId == id);
        if (c is null) return NotFound();

        return new ConversationDetailDto(c.ConversationId, c.CustomerId, c.Customer!.CompanyName,
            c.Contact?.Name, c.Status,
            c.Messages.OrderBy(m => m.Timestamp)
                .Select(m => new MessageDto(m.MessageId, m.Direction, m.SenderPhone,
                    m.MessageText, m.Status, m.Timestamp)).ToList());
    }

    // Salesperson replies from the CRM -> sent through IWhatsAppService (mock for now)
    [HttpPost("{id:int}/messages")]
    public async Task<ActionResult<MessageDto>> Send(int id, SendMessageRequest r, CancellationToken ct)
    {
        var c = await db.Conversations.Include(x => x.Customer).Include(x => x.Contact)
            .FirstOrDefaultAsync(x => x.ConversationId == id, ct);
        if (c is null) return NotFound();

        var phone = c.Contact?.Phone ?? c.Customer?.Phone;
        if (string.IsNullOrWhiteSpace(phone))
            return BadRequest("No phone number on the contact or customer.");

        var externalId = await whatsApp.SendMessageAsync(phone, r.Text, ct);

        var m = new Message
        {
            ConversationId = id,
            Direction = "Outbound",
            SenderPhone = phone,
            MessageText = r.Text,
            Status = "Sent",
            ExternalMessageId = externalId
        };
        db.Messages.Add(m);
        db.AuditLogs.Add(new AuditLog { Entity = "Message", EntityId = id.ToString(), Action = "Sent" });
        await db.SaveChangesAsync(ct);

        return new MessageDto(m.MessageId, m.Direction, m.SenderPhone, m.MessageText, m.Status, m.Timestamp);
    }
}
