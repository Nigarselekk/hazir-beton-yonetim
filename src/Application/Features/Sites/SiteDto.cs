namespace HazirBeton.Application.Features.Sites;

public record SiteDto(
    Guid Id,
    string Name,
    string? Address,
    Guid CustomerId,
    string CustomerCompanyName,
    DateTime CreatedAt
);
