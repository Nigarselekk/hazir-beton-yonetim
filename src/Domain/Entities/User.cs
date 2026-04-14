using HazirBeton.Domain.Enums;

namespace HazirBeton.Domain.Entities;

// Placeholder entity — authentication and role enforcement are implemented in Milestone 3.
public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
}
