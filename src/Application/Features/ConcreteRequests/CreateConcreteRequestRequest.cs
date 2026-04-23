namespace HazirBeton.Application.Features.ConcreteRequests;

public record CreateConcreteRequestRequest(
    Guid CustomerId,
    Guid SiteId,
    string RequesterName,
    string CompanyContactPhone,
    string SiteContactPhone,
    string MaterialType,
    decimal RequestedQuantity,
    decimal UnitPrice,
    string WaybillType,
    string DeliveryMethod,
    DateTime RequestedDateTime,
    string? Note
);
