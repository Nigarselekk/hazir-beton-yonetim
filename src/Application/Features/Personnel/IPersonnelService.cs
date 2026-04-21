namespace HazirBeton.Application.Features.Personnel;

public interface IPersonnelService
{
    Task<List<PersonnelDto>> GetAllAsync(bool? isActive = null);
    Task<PersonnelDto?> GetByIdAsync(Guid id);
    Task<PersonnelDto> CreateAsync(CreatePersonnelRequest request);
    Task<PersonnelDto?> UpdateAsync(Guid id, UpdatePersonnelRequest request);
    Task<bool> DeactivateAsync(Guid id);
    Task<bool> ReactivateAsync(Guid id);
}
