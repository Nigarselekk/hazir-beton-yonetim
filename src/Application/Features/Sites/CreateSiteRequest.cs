namespace HazirBeton.Application.Features.Sites;

public record CreateSiteRequest(
    Guid CustomerId,
    string Name,
    string? Address
);
