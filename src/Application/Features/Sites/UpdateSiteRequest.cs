namespace HazirBeton.Application.Features.Sites;

public record UpdateSiteRequest(
    string Name,
    string? Address
);
