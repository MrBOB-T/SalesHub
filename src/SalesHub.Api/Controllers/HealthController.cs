using Microsoft.AspNetCore.Mvc;
using SalesHub.Infrastructure.Persistence;

namespace SalesHub.Api.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController(SalesHubDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        bool connected;
        try { connected = await db.Database.CanConnectAsync(); }
        catch { connected = false; }

        return Ok(new
        {
            application = "Kolok SalesHub",
            api = "Running",
            database = connected ? "Connected" : "Not connected"
        });
    }
}
