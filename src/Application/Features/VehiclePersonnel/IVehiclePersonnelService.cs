namespace HazirBeton.Application.Features.VehiclePersonnel;

public interface IVehiclePersonnelService
{
    Task<List<VehiclePersonnelDto>> GetByVehicleAsync(Guid vehicleId);
    Task<VehiclePersonnelDto> AssignAsync(Guid vehicleId, AssignPersonnelRequest request);
    Task<VehiclePersonnelDto?> UpdateRoleAsync(Guid vehicleId, Guid personnelId, UpdateAssignmentTypeRequest request);
    Task<bool> RemoveAsync(Guid vehicleId, Guid personnelId);
}
