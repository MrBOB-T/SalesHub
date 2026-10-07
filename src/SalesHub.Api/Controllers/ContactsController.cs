using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesHub.Application.Contacts;
using SalesHub.Domain.Entities;
using SalesHub.Infrastructure.Persistence;

namespace SalesHub.Api.Controllers;

[ApiController]
[Route("api/customers/{customerId:int}/contacts")]
public class ContactsController(SalesHubDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ContactDto>>> GetAll(int customerId)
    {
        if (!await db.Customers.AnyAsync(c => c.CustomerId == customerId)) return NotFound();

        return await db.Contacts.AsNoTracking()
            .Where(x => x.CustomerId == customerId)
            .OrderBy(x => x.Name)
            .Select(x => new ContactDto(x.ContactId, x.CustomerId, x.Name, x.Phone, x.Email, x.Position))
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<ContactDto>> Create(int customerId, CreateContactRequest r)
    {
        if (!await db.Customers.AnyAsync(c => c.CustomerId == customerId))
            return NotFound($"Customer {customerId} not found.");

        var x = new Contact { CustomerId = customerId, Name = r.Name, Phone = r.Phone, Email = r.Email, Position = r.Position };
        db.Contacts.Add(x);
        db.AuditLogs.Add(new AuditLog { Entity = "Contact", EntityId = customerId.ToString(), Action = "Created" });
        await db.SaveChangesAsync();

        return Created($"/api/customers/{customerId}/contacts/{x.ContactId}",
            new ContactDto(x.ContactId, x.CustomerId, x.Name, x.Phone, x.Email, x.Position));
    }
}
