using UserManagement.Api.Contracts.Requests;
using UserManagement.Api.Contracts.Responses;

namespace UserManagement.Api.Services;

public interface IUserService
{
    Task<UserResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken);
}