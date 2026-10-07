
using Microsoft.Extensions.Logging;
using SalesHub.Application.Messaging;

namespace SalesHub.Infrastructure.Messaging;

public class MockWhatsAppService(ILogger<MockWhatsAppService> logger) : IWhatsAppService
{
    public Task<string> SendMessageAsync(string phoneNumber, string message, CancellationToken ct)
    {
        var id = $"mock-out-{Guid.NewGuid():N}";
        logger.LogInformation("[MOCK WhatsApp] To {Phone}: {Message} ({Id})", phoneNumber, message, id);
        return Task.FromResult(id);
    }
}