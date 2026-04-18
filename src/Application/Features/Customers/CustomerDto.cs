namespace HazirBeton.Application.Features.Customers;

public record CustomerDto(
    Guid Id,
    string CompanyName,
    string CommercialCode,
    string? IdentityOrTaxNumber,
    string Phone,
    string? Address,
    string? Notes,
    DateTime CreatedAt
);
