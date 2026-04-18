using HazirBeton.Application.Features.Vehicles;
using HazirBeton.Domain.Entities;
using HazirBeton.Domain.Enums;
using HazirBeton.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HazirBeton.Infrastructure.Services;

public class VehicleService : IVehicleService
{
    private const int MaintenanceAlertDays = 7;

    private readonly AppDbContext _context;

    public VehicleService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<VehicleDto>> GetAllAsync(VehicleStatus? status = null)
    {
        var query = _context.Vehicles.AsQueryable();

        if (status.HasValue)
            query = query.Where(v => v.Status == status.Value);

        return await query
            .OrderBy(v => v.Plate)
            .Select(v => ToDto(v))
            .ToListAsync();
    }

    public async Task<List<VehicleDto>> GetMaintenanceAlertsAsync(int? days = null)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var threshold = today.AddDays(days ?? MaintenanceAlertDays);

        return await _context.Vehicles
            .Where(v => v.Status == VehicleStatus.UnderMaintenance ||
                        (v.NextMaintenanceDate.HasValue && v.NextMaintenanceDate.Value <= threshold))
            .OrderBy(v => v.NextMaintenanceDate)
            .Select(v => ToDto(v))
            .ToListAsync();
    }

    public async Task<VehicleDto?> GetByIdAsync(Guid id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        return vehicle is null ? null : ToDto(vehicle);
    }

    public async Task<VehicleDto> CreateAsync(CreateVehicleRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Plate))
            throw new ArgumentException("Plate is required.");

        var plate = request.Plate.Trim().ToUpperInvariant();

        if (await _context.Vehicles.AnyAsync(v => v.Plate == plate))
            throw new ArgumentException($"Plate '{plate}' is already registered.");

        var vehicle = new Vehicle
        {
            Id = Guid.NewGuid(),
            Plate = plate,
            Type = request.Type,
            Status = VehicleStatus.Active,
            LastMaintenanceDate = request.LastMaintenanceDate,
            NextMaintenanceDate = request.NextMaintenanceDate,
            CreatedAt = DateTime.UtcNow
        };

        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();

        return ToDto(vehicle);
    }

    public async Task<VehicleDto?> UpdateAsync(Guid id, UpdateVehicleRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Plate))
            throw new ArgumentException("Plate is required.");

        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle is null) return null;

        var plate = request.Plate.Trim().ToUpperInvariant();

        if (await _context.Vehicles.AnyAsync(v => v.Plate == plate && v.Id != id))
            throw new ArgumentException($"Plate '{plate}' is already registered to another vehicle.");

        vehicle.Plate = plate;
        vehicle.Type = request.Type;
        vehicle.Status = request.Status;
        vehicle.LastMaintenanceDate = request.LastMaintenanceDate;
        vehicle.NextMaintenanceDate = request.NextMaintenanceDate;
        vehicle.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return ToDto(vehicle);
    }

    public async Task<bool> DeactivateAsync(Guid id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle is null) return false;

        vehicle.Status = VehicleStatus.Passive;
        vehicle.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    private static VehicleDto ToDto(Vehicle v)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var threshold = today.AddDays(MaintenanceAlertDays);

        var alert = v.Status == VehicleStatus.UnderMaintenance ||
                    (v.NextMaintenanceDate.HasValue && v.NextMaintenanceDate.Value <= threshold);

        return new VehicleDto(
            v.Id,
            v.Plate,
            v.Type,
            GetTypeLabel(v.Type),
            v.Status,
            GetStatusLabel(v.Status),
            v.LastMaintenanceDate,
            v.NextMaintenanceDate,
            alert,
            v.CreatedAt
        );
    }

    private static string GetTypeLabel(VehicleType type) => type switch
    {
        VehicleType.ConcreteMixer  => "Transmikser",
        VehicleType.Pump           => "Pompa",
        VehicleType.Excavator      => "Ekskavatör",
        VehicleType.SiteVehicle    => "Şantiye Aracı",
        VehicleType.ServiceVehicle => "Servis Aracı",
        _                          => type.ToString()
    };

    private static string GetStatusLabel(VehicleStatus status) => status switch
    {
        VehicleStatus.Active           => "Aktif",
        VehicleStatus.UnderMaintenance => "Bakımda",
        VehicleStatus.Passive          => "Pasif",
        VehicleStatus.OutOfSystem      => "Sistem Dışı",
        _                              => status.ToString()
    };
}
