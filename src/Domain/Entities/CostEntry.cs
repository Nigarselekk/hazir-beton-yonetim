using HazirBeton.Domain.Enums;

namespace HazirBeton.Domain.Entities;

public class CostEntry : BaseEntity
{
    public DateOnly Date { get; set; }
    public CostItemType CostItemType { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
    public string? Description { get; set; }
}
