using HazirBeton.Domain.Enums;

namespace HazirBeton.Application.Features.ConcreteRequests;

public record ConcreteRequestFilterRequest(
    ConcreteRequestStatus? Status = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    Guid? CustomerId = null,
    Guid? SiteId = null,
    int Page = 1,
    int PageSize = 20
);
