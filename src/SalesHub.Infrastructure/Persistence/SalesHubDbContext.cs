using Microsoft.EntityFrameworkCore;
using SalesHub.Domain.Entities;

namespace SalesHub.Infrastructure.Persistence;

public class SalesHubDbContext(DbContextOptions<SalesHubDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<User> Users => Set<User>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<AuditLog>().HasKey(a => a.AuditId);

        b.Entity<Customer>(e =>
        {
            e.Property(x => x.AccountNumber).HasMaxLength(30).IsRequired();
            e.HasIndex(x => x.AccountNumber).IsUnique();
            e.Property(x => x.CompanyName).HasMaxLength(200).IsRequired();
            e.Property(x => x.Status).HasMaxLength(20);
        });

        b.Entity<User>(e =>
        {
            e.Property(x => x.Email).HasMaxLength(200).IsRequired();
            e.HasIndex(x => x.Email).IsUnique();
        });

        b.Entity<Message>(e =>
        {
            e.Property(x => x.Direction).HasMaxLength(10);
            e.HasIndex(x => x.ExternalMessageId).IsUnique()
             .HasFilter("[ExternalMessageId] IS NOT NULL");
        });

        // Keep CRM history: no cascade deletes (also avoids SQL Server multiple cascade path errors)
        foreach (var fk in b.Model.GetEntityTypes().SelectMany(t => t.GetForeignKeys()))
            fk.DeleteBehavior = DeleteBehavior.Restrict;

        // Seed test data (test data only, per POC governance)
        b.Entity<User>().HasData(
            new User { UserId = 1, Name = "Sales Manager", Email = "manager@saleshub.local", Role = "Manager" },
            new User { UserId = 2, Name = "Test Salesperson", Email = "sales1@saleshub.local", Role = "Sales" });

        b.Entity<Customer>().HasData(
            new Customer { CustomerId = 1, AccountNumber = "TEST001", CompanyName = "ABC Office Supplies",
                Phone = "+27000000001", Email = "info@abc.test", AssignedUserId = 2, Status = "Active",
                CreatedAt = new DateTime(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc) });

        b.Entity<Contact>().HasData(
            new Contact { ContactId = 1, CustomerId = 1, Name = "John Smith",
                Phone = "+27000000002", Email = "john@abc.test", Position = "Buyer" });
    }
}
