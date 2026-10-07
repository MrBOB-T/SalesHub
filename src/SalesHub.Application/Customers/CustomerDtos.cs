using System.ComponentModel.DataAnnotations;

namespace SalesHub.Application.Customers;

public record CustomerDto(int CustomerId, string AccountNumber, string CompanyName,
    string? Phone, string? Email, int? AssignedUserId, string Status);

public record CreateCustomerRequest(
    [Required, MaxLength(30)] string AccountNumber,
    [Required, MaxLength(200)] string CompanyName,
    string? Phone, string? Email, int? AssignedUserId);

public record UpdateCustomerRequest(
    [Required, MaxLength(200)] string CompanyName,
    string? Phone, string? Email, int? AssignedUserId,
    [Required] string Status);
