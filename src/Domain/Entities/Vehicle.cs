using HazirBeton.Domain.Enums;

namespace HazirBeton.Domain.Entities;

public class Vehicle : BaseEntity
{
    public string Plate { get; set; } = string.Empty;
    public VehicleType Type { get; set; }
    public VehicleStatus Status { get; set; }
    public DateOnly? LastMaintenanceDate { get; set; }
    public DateOnly? NextMaintenanceDate { get; set; }

    public ICollection<VehiclePersonnel> VehiclePersonnel { get; set; } = new List<VehiclePersonnel>();
    public ICollection<ConcreteRequest> ConcreteRequests { get; set; } = new List<ConcreteRequest>();
}
