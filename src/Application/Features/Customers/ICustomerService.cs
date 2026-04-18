namespace HazirBeton.Application.Features.Customers;

public interface ICustomerService
{
    Task<List<CustomerDto>> GetAllAsync();
    Task<CustomerDto?> GetByIdAsync(Guid id);
    Task<CustomerDto> CreateAsync(CreateCustomerRequest request);
    Task<CustomerDto?> UpdateAsync(Guid id, UpdateCustomerRequest request);
    Task<bool> DeleteAsync(Guid id);
}
