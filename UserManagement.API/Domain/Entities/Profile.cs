namespace UserManagement.Api.Domain.Entities;

public class Profile
{
    public int UserId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    public User User { get; set; } = null!;
}