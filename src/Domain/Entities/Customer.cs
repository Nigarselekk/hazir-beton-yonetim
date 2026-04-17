namespace HazirBeton.Domain.Entities;

public class Customer : BaseEntity
{
    public string CompanyName { get; set; } = string.Empty;
    public string CommercialCode { get; set; } = string.Empty;
    public string? IdentityOrTaxNumber { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Notes { get; set; }

    public ICollection<Site> Sites { get; set; } = new List<Site>();
}
