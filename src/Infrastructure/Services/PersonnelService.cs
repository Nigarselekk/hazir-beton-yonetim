using HazirBeton.Application.Features.Personnel;
using HazirBeton.Domain.Entities;
using HazirBeton.Domain.Enums;
using HazirBeton.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HazirBeton.Infrastructure.Services;

public class PersonnelService : IPersonnelService
{
    private readonly AppDbContext _context;

    public PersonnelService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<PersonnelDto>> GetAllAsync(bool? isActive = null)
    {
        var query = _context.Personnel.AsQueryable();

        if (isActive.HasValue)
            query = query.Where(p => p.IsActive == isActive.Value);

        var personnel = await query.OrderBy(p => p.FullName).ToListAsync();
        return personnel.Select(ToDto).ToList();
    }

    public async Task<PersonnelDto?> GetByIdAsync(Guid id)
    {
        var personnel = await _context.Personnel.FindAsync(id);
        return personnel is null ? null : ToDto(personnel);
    }

    public async Task<PersonnelDto> CreateAsync(CreatePersonnelRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
            throw new ArgumentException("FullName is required.");
        if (string.IsNullOrWhiteSpace(request.Phone))
            throw new ArgumentException("Phone is required.");

        var personnel = new Domain.Entities.Personnel
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName.Trim(),
            Phone = request.Phone.Trim(),
            Type = request.Type,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Personnel.Add(personnel);
        await _context.SaveChangesAsync();

        return ToDto(personnel);
    }

    public async Task<PersonnelDto?> UpdateAsync(Guid id, UpdatePersonnelRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
            throw new ArgumentException("FullName is required.");
        if (string.IsNullOrWhiteSpace(request.Phone))
            throw new ArgumentException("Phone is required.");

        var personnel = await _context.Personnel.FindAsync(id);
        if (personnel is null) return null;

        personnel.FullName = request.FullName.Trim();
        personnel.Phone = request.Phone.Trim();
        personnel.Type = request.Type;
        personnel.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return ToDto(personnel);
    }

    public async Task<bool> DeactivateAsync(Guid id)
    {
        var personnel = await _context.Personnel.FindAsync(id);
        if (personnel is null) return false;

        personnel.IsActive = false;
        personnel.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReactivateAsync(Guid id)
    {
        var personnel = await _context.Personnel.FindAsync(id);
        if (personnel is null) return false;

        personnel.IsActive = true;
        personnel.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    private static PersonnelDto ToDto(Domain.Entities.Personnel p) =>
        new(p.Id, p.FullName, p.Phone, p.Type, GetTypeLabel(p.Type), p.IsActive, p.CreatedAt);

    private static string GetTypeLabel(PersonnelType type) => type switch
    {
        PersonnelType.MixerDriver     => "Mikser Şoförü",
        PersonnelType.PumpOperator    => "Pompa Operatörü",
        PersonnelType.FieldPersonnel  => "Saha Personeli",
        PersonnelType.ServiceDriver   => "Servis Şoförü",
        _                             => type.ToString()
    };
}
