namespace HazirBeton.Application.Features.Sites;

public interface ISiteService
{
    Task<List<SiteDto>> GetAllAsync(Guid? customerId = null);
    Task<SiteDto?> GetByIdAsync(Guid id);
    Task<SiteDto> CreateAsync(CreateSiteRequest request);
    Task<SiteDto?> UpdateAsync(Guid id, UpdateSiteRequest request);
    Task<bool> DeleteAsync(Guid id);
}
