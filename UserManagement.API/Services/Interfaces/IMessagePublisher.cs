using UserManagement.Api.Domain.Entities;

namespace UserManagement.Api.Services;

public interface IMessagePublisher
{
    Task PublishUserCreatedAsync(User user, CancellationToken cancellationToken);
}