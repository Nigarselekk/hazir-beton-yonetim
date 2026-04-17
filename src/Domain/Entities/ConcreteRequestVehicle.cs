namespace HazirBeton.Domain.Entities;

public class ConcreteRequestVehicle : BaseEntity
{
    public Guid ConcreteRequestId { get; set; }
    public ConcreteRequest ConcreteRequest { get; set; } = null!;

    public Guid VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;
}
