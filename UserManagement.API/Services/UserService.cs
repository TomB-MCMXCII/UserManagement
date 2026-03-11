using Microsoft.EntityFrameworkCore;
using UserManagement.Api.Contracts.Requests;
using UserManagement.Api.Contracts.Responses;
using UserManagement.Api.Data;
using UserManagement.Api.Domain.Entities;

namespace UserManagement.Api.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _dbContext;
    private readonly IMessagePublisher _messagePublisher;
    private readonly ILogger<UserService> _logger;

    public UserService(
        AppDbContext dbContext,
        IMessagePublisher messagePublisher,
        ILogger<UserService> logger)
    {
        _dbContext = dbContext;
        _messagePublisher = messagePublisher;
        _logger = logger;
    }

    public async Task<UserResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .Include(x => x.Profile)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (user is null)
        {
            _logger.LogInformation("User with id {UserId} was not found.", id);
            return null;
        }

        return MapToResponse(user);
    }

    public async Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            Profile = new Profile
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth
            }
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User with id {UserId} was created.", user.Id);

        await _messagePublisher.PublishUserCreatedAsync(user, cancellationToken);

        return MapToResponse(user);
    }

    private static UserResponse MapToResponse(User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Profile = new ProfileResponse
            {
                FirstName = user.Profile.FirstName,
                LastName = user.Profile.LastName,
                DateOfBirth = user.Profile.DateOfBirth
            }
        };
    }
}