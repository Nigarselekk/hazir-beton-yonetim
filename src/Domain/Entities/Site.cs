namespace HazirBeton.Domain.Entities;

public class Site : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public ICollection<ConcreteRequest> ConcreteRequests { get; set; } = new List<ConcreteRequest>();
}
