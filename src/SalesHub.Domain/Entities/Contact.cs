namespace SalesHub.Domain.Entities;

public class Contact
{
    public int ContactId { get; set; }
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Position { get; set; }
}
