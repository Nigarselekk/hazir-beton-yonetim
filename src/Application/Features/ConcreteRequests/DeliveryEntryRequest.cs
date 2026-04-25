namespace HazirBeton.Application.Features.ConcreteRequests;

public record DeliveryEntryRequest(
    decimal DeliveredQuantity,
    uint RowVersion
);
