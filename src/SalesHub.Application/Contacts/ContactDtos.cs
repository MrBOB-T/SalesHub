using System.ComponentModel.DataAnnotations;

namespace SalesHub.Application.Contacts;

public record ContactDto(int ContactId, int CustomerId, string Name,
    string? Phone, string? Email, string? Position);

public record CreateContactRequest(
    [Required, MaxLength(150)] string Name,
    string? Phone, string? Email, string? Position);
