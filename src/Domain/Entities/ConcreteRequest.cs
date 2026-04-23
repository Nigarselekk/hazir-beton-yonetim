using HazirBeton.Domain.Enums;

namespace HazirBeton.Domain.Entities;

public class ConcreteRequest : BaseEntity
{
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public Guid SiteId { get; set; }
    public Site Site { get; set; } = null!;

    public string RequesterName { get; set; } = string.Empty;
    public string CompanyContactPhone { get; set; } = string.Empty;
    public string SiteContactPhone { get; set; } = string.Empty;

    public string MaterialType { get; set; } = string.Empty;

    // Requested quantity: what the customer asked for (never overwritten)
    public decimal RequestedQuantity { get; set; }

    public decimal UnitPrice { get; set; }

    // Stored for reporting; equals UnitPrice × RequestedQuantity at creation
    public decimal TotalAmount { get; set; }

    public string WaybillType { get; set; } = string.Empty;
    public string DeliveryMethod { get; set; } = string.Empty;

    // Requested date/time: the customer's original ask (never overwritten)
    public DateTime RequestedDateTime { get; set; }

    // Approved date/time: set by the manager on approval — separate from RequestedDateTime
    public DateTime? ApprovedAppointmentDateTime { get; set; }

    // Delivered quantity: actual m³ entered at day-end — separate from RequestedQuantity
    public decimal? DeliveredQuantity { get; set; }

    public ConcreteRequestStatus Status { get; set; } = ConcreteRequestStatus.PendingApproval;

    public Guid? ApprovedById { get; set; }
    public User? ApprovedBy { get; set; }

    public Guid? AssignedById { get; set; }
    public User? AssignedBy { get; set; }

    public Guid? CreatedById { get; set; }
    public User? CreatedBy { get; set; }

    public string? Note { get; set; }
    public string? CancellationReason { get; set; }

    public ICollection<ConcreteRequestVehicle> ConcreteRequestVehicles { get; set; } = new List<ConcreteRequestVehicle>();
    public ICollection<SmsLog> SmsLogs { get; set; } = new List<SmsLog>();
}
