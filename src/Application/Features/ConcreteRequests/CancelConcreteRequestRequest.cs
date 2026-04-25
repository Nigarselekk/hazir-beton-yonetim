namespace HazirBeton.Application.Features.ConcreteRequests;

public record CancelConcreteRequestRequest(
    string? Reason,
    uint RowVersion
);
