using HazirBeton.Domain.Enums;

namespace HazirBeton.Application.Features.Vehicles;

public interface IVehicleService
{
    Task<List<VehicleDto>> GetAllAsync(VehicleStatus? status = null);
    Task<List<VehicleDto>> GetMaintenanceAlertsAsync(int? days = null);
    Task<VehicleDto?> GetByIdAsync(Guid id);
    Task<VehicleDto> CreateAsync(CreateVehicleRequest request);
    Task<VehicleDto?> UpdateAsync(Guid id, UpdateVehicleRequest request);
    Task<bool> DeactivateAsync(Guid id);
}
