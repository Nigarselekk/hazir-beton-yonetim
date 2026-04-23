using HazirBeton.Domain.Enums;

namespace HazirBeton.Application.Features.Users;

public interface IUserService
{
    Task<List<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<UserDto> CreateAsync(CreateUserRequest request);
    Task<UserDto?> UpdateAsync(Guid id, UpdateUserRequest request);
    Task<bool> DeactivateAsync(Guid id);
    Task<bool> ReactivateAsync(Guid id);
    Task SetPermissionsAsync(Guid userId, List<Permission> permissions);
}
