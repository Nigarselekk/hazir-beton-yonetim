using HazirBeton.Application.Exceptions;
using HazirBeton.Application.Features.Users;
using HazirBeton.Domain.Entities;
using HazirBeton.Domain.Enums;
using HazirBeton.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HazirBeton.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        return await _db.Users
            .Include(u => u.Permissions)
            .OrderBy(u => u.FullName)
            .Select(u => ToDto(u))
            .ToListAsync();
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _db.Users.Include(u => u.Permissions).FirstOrDefaultAsync(u => u.Id == id);
        return user is null ? null : ToDto(user);
    }

    public async Task<UserDto> CreateAsync(CreateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
            throw new ArgumentException("Kullanıcı adı boş olamaz.");

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            throw new ArgumentException("Şifre en az 6 karakter olmalıdır.");

        var exists = await _db.Users.AnyAsync(u => u.Username == request.Username);
        if (exists)
            throw new ConflictException($"'{request.Username}' kullanıcı adı zaten kullanımda.");

        var permissions = (request.CustomPermissions is { Count: > 0 })
            ? request.CustomPermissions
            : DefaultPermissions.For(request.Role);

        // HeadManager permissions are derived from role — no need to store them
        var permissionsToStore = request.Role == UserRole.HeadManager
            ? new List<Permission>()
            : permissions;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username.Trim(),
            FullName = request.FullName.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        user.Permissions = permissionsToStore.Select(p => new UserPermission
        {
            UserId = user.Id,
            Permission = p
        }).ToList();

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return ToDto(user);
    }

    public async Task<UserDto?> UpdateAsync(Guid id, UpdateUserRequest request)
    {
        var user = await _db.Users.Include(u => u.Permissions).FirstOrDefaultAsync(u => u.Id == id);
        if (user is null) return null;

        if (!string.IsNullOrWhiteSpace(request.FullName))
            user.FullName = request.FullName.Trim();

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            if (request.Password.Length < 6)
                throw new ArgumentException("Şifre en az 6 karakter olmalıdır.");
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            // Invalidate refresh token on password change
            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;
        }

        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return ToDto(user);
    }

    public async Task<bool> DeactivateAsync(Guid id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is null) return false;

        user.IsActive = false;
        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReactivateAsync(Guid id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user is null) return false;

        user.IsActive = true;
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task SetPermissionsAsync(Guid userId, List<Permission> permissions)
    {
        var user = await _db.Users.Include(u => u.Permissions).FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
            throw new ArgumentException("Kullanıcı bulunamadı.");

        // HeadManager permissions come from role — cannot be overridden via this endpoint
        if (user.Role == UserRole.HeadManager)
            throw new ArgumentException("HeadManager yetkilerini bu endpoint üzerinden değiştiremezsiniz.");

        _db.UserPermissions.RemoveRange(user.Permissions);

        var newPermissions = permissions.Select(p => new UserPermission
        {
            UserId = userId,
            Permission = p
        }).ToList();

        _db.UserPermissions.AddRange(newPermissions);
        await _db.SaveChangesAsync();
    }

    private static UserDto ToDto(User u) => new(
        u.Id,
        u.Username,
        u.FullName,
        u.Role.ToString(),
        u.IsActive,
        u.Role == UserRole.HeadManager
            ? Enum.GetValues<Permission>().Select(p => p.ToString()).ToList()
            : u.Permissions.Select(p => p.Permission.ToString()).ToList(),
        u.CreatedAt
    );
}
