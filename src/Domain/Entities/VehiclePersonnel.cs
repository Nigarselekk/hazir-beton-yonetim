using HazirBeton.Domain.Enums;

namespace HazirBeton.Domain.Entities;

public class VehiclePersonnel : BaseEntity
{
    public Guid VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;

    public Guid PersonnelId { get; set; }
    public Personnel Personnel { get; set; } = null!;

    public PersonnelAssignmentType AssignmentType { get; set; }
}
