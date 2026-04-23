using HazirBeton.Domain.Enums;

namespace HazirBeton.Domain.Entities;

public class UserPermission
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Permission Permission { get; set; }
}
