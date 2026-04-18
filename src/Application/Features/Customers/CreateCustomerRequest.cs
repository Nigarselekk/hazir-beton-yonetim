namespace HazirBeton.Application.Features.Customers;

public record CreateCustomerRequest(
    string CompanyName,
    string CommercialCode,
    string? IdentityOrTaxNumber,
    string Phone,
    string? Address,
    string? Notes
);
