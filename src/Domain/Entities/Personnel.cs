using HazirBeton.Domain.Enums;

namespace HazirBeton.Domain.Entities;

public class Personnel : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public PersonnelType Type { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<VehiclePersonnel> VehiclePersonnel { get; set; } = new List<VehiclePersonnel>();
}
