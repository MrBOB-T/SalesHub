namespace SalesHub.Domain.Entities;

public class User
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "Sales"; // Sales, Manager, Admin
    public bool IsActive { get; set; } = true;
}
