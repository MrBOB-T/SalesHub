using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesHub.Infrastructure.Persistence;

namespace SalesHub.Api.Controllers;

[ApiController]
[Route("api/activities")]
public class ActivitiesController(SalesHubDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? userId, [FromQuery] bool openOnly = true)
    {
        var q = db.Activities.AsNoTracking();
        if (userId is not null) q = q.Where(a => a.UserId == userId);
        if (openOnly) q = q.Where(a => a.CompletedDate == null);

        return Ok(await q.OrderBy(a => a.DueDate)
            .Select(a => new { a.ActivityId, a.CustomerId, a.UserId, a.Type, a.Description, a.DueDate, a.CompletedDate })
            .ToListAsync());
    }
}