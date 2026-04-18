namespace HazirBeton.Application.Features.Customers;

public record UpdateCustomerRequest(
    string CompanyName,
    string CommercialCode,
    string? IdentityOrTaxNumber,
    string Phone,
    string? Address,
    string? Notes
);
