namespace UserManagement.Api.Contracts.Responses;

public class UserResponse
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public ProfileResponse Profile { get; set; } = new();
}