namespace SalesHub.Application.Messaging;

public interface IWhatsAppService
{
    /// <summary>Sends a message and returns the provider's message id.</summary>
    Task<string> SendMessageAsync(string phoneNumber, string message, CancellationToken ct);
}