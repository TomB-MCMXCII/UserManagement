using UserManagement.Api.Domain.Entities;

namespace UserManagement.Api.Domain.Entities;

public class User
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public Profile Profile { get; set; } = null!;
}