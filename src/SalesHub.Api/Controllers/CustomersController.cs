using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesHub.Application.Customers;
using SalesHub.Domain.Entities;
using SalesHub.Infrastructure.Persistence;

namespace SalesHub.Api.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController(SalesHubDbContext db) : ControllerBase
{
    private static CustomerDto ToDto(Customer c) => new(c.CustomerId, c.AccountNumber,
        c.CompanyName, c.Phone, c.Email, c.AssignedUserId, c.Status);

    [HttpGet]
    public async Task<IEnumerable<CustomerDto>> GetAll([FromQuery] string? search)
    {
        var q = db.Customers.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(c => c.CompanyName.Contains(search) || c.AccountNumber.Contains(search));

        return await q.OrderBy(c => c.CompanyName)
            .Select(c => new CustomerDto(c.CustomerId, c.AccountNumber, c.CompanyName,
                c.Phone, c.Email, c.AssignedUserId, c.Status))
            .ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerDto>> Get(int id)
    {
        var c = await db.Customers.FindAsync(id);
        return c is null ? NotFound() : ToDto(c);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerDto>> Create(CreateCustomerRequest r)
    {
        if (await db.Customers.AnyAsync(c => c.AccountNumber == r.AccountNumber))
            return Conflict($"Account {r.AccountNumber} already exists.");

        var c = new Customer
        {
            AccountNumber = r.AccountNumber,
            CompanyName = r.CompanyName,
            Phone = r.Phone,
            Email = r.Email,
            AssignedUserId = r.AssignedUserId
        };
        db.Customers.Add(c);
        db.AuditLogs.Add(new AuditLog { Entity = "Customer", EntityId = r.AccountNumber, Action = "Created" });
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = c.CustomerId }, ToDto(c));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateCustomerRequest r)
    {
        var c = await db.Customers.FindAsync(id);
        if (c is null) return NotFound();

        c.CompanyName = r.CompanyName;
        c.Phone = r.Phone;
        c.Email = r.Email;
        c.AssignedUserId = r.AssignedUserId;
        c.Status = r.Status;
        db.AuditLogs.Add(new AuditLog { Entity = "Customer", EntityId = id.ToString(), Action = "Updated" });
        await db.SaveChangesAsync();
        return NoContent();
    }
}
