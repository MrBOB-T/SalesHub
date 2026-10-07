using Microsoft.EntityFrameworkCore;
using SalesHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using SalesHub.Application.Messaging;
using SalesHub.Infrastructure.Messaging;
using SalesHub.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SalesHubDbContext>(o =>
o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IWhatsAppService, MockWhatsAppService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Apply any pending migrations automatically in Development
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<SalesHubDbContext>();
        try { db.Database.Migrate(); }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "Database migration failed. Check the connection string in appsettings.json.");
        }
    }

    app.MapOpenApi();                                   // /openapi/v1.json
    app.UseSwaggerUI(o =>
    {
        o.SwaggerEndpoint("/openapi/v1.json", "Kolok SalesHub API v1");
        o.RoutePrefix = "swagger";                      // /swagger
    });
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
