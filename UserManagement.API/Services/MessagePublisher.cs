using System.Text.Json;
using UserManagement.Api.Domain.Entities;

namespace UserManagement.Api.Services;

public class MessagePublisherStub : IMessagePublisher
{
    private readonly ILogger<MessagePublisherStub> _logger;

    public MessagePublisherStub(ILogger<MessagePublisherStub> logger)
    {
        _logger = logger;
    }

    public Task PublishUserCreatedAsync(User user, CancellationToken cancellationToken)
    {
        var message = new
        {
            UserId = user.Id,
            user.Username,
            user.Email
        };

        var json = JsonSerializer.Serialize(message);

        _logger.LogInformation("Publishing user created message: {Message}", json);

        return Task.CompletedTask;
    }
}