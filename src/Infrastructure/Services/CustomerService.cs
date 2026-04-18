using HazirBeton.Application.Exceptions;
using HazirBeton.Application.Features.Customers;
using HazirBeton.Domain.Entities;
using HazirBeton.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HazirBeton.Infrastructure.Services;

public class CustomerService : ICustomerService
{
    private readonly AppDbContext _context;

    public CustomerService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CustomerDto>> GetAllAsync()
    {
        return await _context.Customers
            .OrderBy(c => c.CompanyName)
            .Select(c => new CustomerDto(
                c.Id, c.CompanyName, c.CommercialCode, c.IdentityOrTaxNumber,
                c.Phone, c.Address, c.Notes, c.CreatedAt))
            .ToListAsync();
    }

    public async Task<CustomerDto?> GetByIdAsync(Guid id)
    {
        var c = await _context.Customers.FindAsync(id);
        if (c is null) return null;

        return new CustomerDto(c.Id, c.CompanyName, c.CommercialCode, c.IdentityOrTaxNumber,
            c.Phone, c.Address, c.Notes, c.CreatedAt);
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CompanyName))
            throw new ArgumentException("CompanyName is required.");
        if (string.IsNullOrWhiteSpace(request.CommercialCode))
            throw new ArgumentException("CommercialCode is required.");
        if (string.IsNullOrWhiteSpace(request.Phone))
            throw new ArgumentException("Phone is required.");

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            CompanyName = request.CompanyName.Trim(),
            CommercialCode = request.CommercialCode.Trim(),
            IdentityOrTaxNumber = request.IdentityOrTaxNumber?.Trim(),
            Phone = request.Phone.Trim(),
            Address = request.Address?.Trim(),
            Notes = request.Notes?.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return new CustomerDto(customer.Id, customer.CompanyName, customer.CommercialCode,
            customer.IdentityOrTaxNumber, customer.Phone, customer.Address, customer.Notes, customer.CreatedAt);
    }

    public async Task<CustomerDto?> UpdateAsync(Guid id, UpdateCustomerRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CompanyName))
            throw new ArgumentException("CompanyName is required.");
        if (string.IsNullOrWhiteSpace(request.CommercialCode))
            throw new ArgumentException("CommercialCode is required.");
        if (string.IsNullOrWhiteSpace(request.Phone))
            throw new ArgumentException("Phone is required.");

        var customer = await _context.Customers.FindAsync(id);
        if (customer is null) return null;

        customer.CompanyName = request.CompanyName.Trim();
        customer.CommercialCode = request.CommercialCode.Trim();
        customer.IdentityOrTaxNumber = request.IdentityOrTaxNumber?.Trim();
        customer.Phone = request.Phone.Trim();
        customer.Address = request.Address?.Trim();
        customer.Notes = request.Notes?.Trim();
        customer.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new CustomerDto(customer.Id, customer.CompanyName, customer.CommercialCode,
            customer.IdentityOrTaxNumber, customer.Phone, customer.Address, customer.Notes, customer.CreatedAt);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var customer = await _context.Customers
            .Include(c => c.Sites)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (customer is null) return false;

        if (customer.Sites.Any())
            throw new ConflictException("Bu müşteriye bağlı şantiyeler bulunduğundan silinemez.");

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
        return true;
    }
}
