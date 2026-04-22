using HazirBeton.Application.Exceptions;
using HazirBeton.Application.Features.VehiclePersonnel;
using HazirBeton.Domain.Enums;
using HazirBeton.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HazirBeton.Infrastructure.Services;

public class VehiclePersonnelService : IVehiclePersonnelService
{
    private readonly AppDbContext _context;

    public VehiclePersonnelService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<VehiclePersonnelDto>> GetByVehicleAsync(Guid vehicleId)
    {
        return await _context.VehiclePersonnel
            .Where(vp => vp.VehicleId == vehicleId)
            .OrderBy(vp => vp.AssignmentType)
            .ThenBy(vp => vp.Personnel.FullName)
            .Select(vp => ToDto(vp))
            .ToListAsync();
    }

    public async Task<VehiclePersonnelDto> AssignAsync(Guid vehicleId, AssignPersonnelRequest request)
    {
        var vehicle = await _context.Vehicles.FindAsync(vehicleId);
        if (vehicle is null)
            throw new ArgumentException("Vehicle not found.");

        var personnel = await _context.Personnel.FindAsync(request.PersonnelId);
        if (personnel is null)
            throw new ArgumentException("Personnel not found.");
        if (!personnel.IsActive)
            throw new ArgumentException("Inactive personnel cannot be assigned.");

        var alreadyAssigned = await _context.VehiclePersonnel
            .AnyAsync(vp => vp.VehicleId == vehicleId && vp.PersonnelId == request.PersonnelId);
        if (alreadyAssigned)
            throw new ConflictException("Bu personel bu araca zaten atanmış.");

        if (request.AssignmentType == PersonnelAssignmentType.Primary)
        {
            var primaryExists = await _context.VehiclePersonnel
                .AnyAsync(vp => vp.VehicleId == vehicleId && vp.AssignmentType == PersonnelAssignmentType.Primary);
            if (primaryExists)
                throw new ConflictException("Bu araçta zaten bir esas personel tanımlı. Önce mevcut esas personelin rolünü değiştirin.");
        }

        var assignment = new Domain.Entities.VehiclePersonnel
        {
            Id = Guid.NewGuid(),
            VehicleId = vehicleId,
            PersonnelId = request.PersonnelId,
            AssignmentType = request.AssignmentType,
            CreatedAt = DateTime.UtcNow
        };

        _context.VehiclePersonnel.Add(assignment);
        await _context.SaveChangesAsync();

        await _context.Entry(assignment).Reference(vp => vp.Personnel).LoadAsync();

        return ToDto(assignment);
    }

    public async Task<VehiclePersonnelDto?> UpdateRoleAsync(Guid vehicleId, Guid personnelId, UpdateAssignmentTypeRequest request)
    {
        var assignment = await _context.VehiclePersonnel
            .Include(vp => vp.Personnel)
            .FirstOrDefaultAsync(vp => vp.VehicleId == vehicleId && vp.PersonnelId == personnelId);

        if (assignment is null) return null;

        if (request.AssignmentType == PersonnelAssignmentType.Primary)
        {
            var primaryExists = await _context.VehiclePersonnel
                .AnyAsync(vp => vp.VehicleId == vehicleId &&
                                vp.AssignmentType == PersonnelAssignmentType.Primary &&
                                vp.PersonnelId != personnelId);
            if (primaryExists)
                throw new ConflictException("Bu araçta zaten bir esas personel tanımlı. Önce mevcut esas personelin rolünü değiştirin.");
        }

        assignment.AssignmentType = request.AssignmentType;
        assignment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return ToDto(assignment);
    }

    public async Task<bool> RemoveAsync(Guid vehicleId, Guid personnelId)
    {
        var assignment = await _context.VehiclePersonnel
            .FirstOrDefaultAsync(vp => vp.VehicleId == vehicleId && vp.PersonnelId == personnelId);

        if (assignment is null) return false;

        _context.VehiclePersonnel.Remove(assignment);
        await _context.SaveChangesAsync();
        return true;
    }

    private static VehiclePersonnelDto ToDto(Domain.Entities.VehiclePersonnel vp) =>
        new(
            vp.Id,
            vp.VehicleId,
            vp.PersonnelId,
            vp.Personnel.FullName,
            vp.Personnel.Phone,
            vp.Personnel.Type,
            GetPersonnelTypeLabel(vp.Personnel.Type),
            vp.AssignmentType,
            GetAssignmentTypeLabel(vp.AssignmentType),
            vp.CreatedAt
        );

    private static string GetPersonnelTypeLabel(PersonnelType type) => type switch
    {
        PersonnelType.MixerDriver    => "Mikser Şoförü",
        PersonnelType.PumpOperator   => "Pompa Operatörü",
        PersonnelType.FieldPersonnel => "Saha Personeli",
        PersonnelType.ServiceDriver  => "Servis Şoförü",
        _                            => type.ToString()
    };

    private static string GetAssignmentTypeLabel(PersonnelAssignmentType type) => type switch
    {
        PersonnelAssignmentType.Primary => "Esas",
        PersonnelAssignmentType.Backup  => "Yedek",
        _                               => type.ToString()
    };
}
