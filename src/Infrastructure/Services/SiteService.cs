using HazirBeton.Application.Exceptions;
using HazirBeton.Application.Features.Sites;
using HazirBeton.Domain.Entities;
using HazirBeton.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HazirBeton.Infrastructure.Services;

public class SiteService : ISiteService
{
    private readonly AppDbContext _context;

    public SiteService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SiteDto>> GetAllAsync(Guid? customerId = null)
    {
        var query = _context.Sites.Include(s => s.Customer).AsQueryable();

        if (customerId.HasValue)
            query = query.Where(s => s.CustomerId == customerId.Value);

        return await query
            .OrderBy(s => s.Name)
            .Select(s => new SiteDto(
                s.Id, s.Name, s.Address,
                s.CustomerId, s.Customer.CompanyName,
                s.CreatedAt))
            .ToListAsync();
    }

    public async Task<SiteDto?> GetByIdAsync(Guid id)
    {
        var s = await _context.Sites
            .Include(s => s.Customer)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (s is null) return null;

        return new SiteDto(s.Id, s.Name, s.Address, s.CustomerId, s.Customer.CompanyName, s.CreatedAt);
    }

    public async Task<SiteDto> CreateAsync(CreateSiteRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Name is required.");

        var customerExists = await _context.Customers.AnyAsync(c => c.Id == request.CustomerId);
        if (!customerExists)
            throw new ArgumentException($"Customer with id '{request.CustomerId}' not found.");

        var site = new Site
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            Name = request.Name.Trim(),
            Address = request.Address?.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _context.Sites.Add(site);
        await _context.SaveChangesAsync();

        var customerName = (await _context.Customers.FindAsync(request.CustomerId))!.CompanyName;
        return new SiteDto(site.Id, site.Name, site.Address, site.CustomerId, customerName, site.CreatedAt);
    }

    public async Task<SiteDto?> UpdateAsync(Guid id, UpdateSiteRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Name is required.");

        var site = await _context.Sites
            .Include(s => s.Customer)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (site is null) return null;

        site.Name = request.Name.Trim();
        site.Address = request.Address?.Trim();
        site.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new SiteDto(site.Id, site.Name, site.Address, site.CustomerId, site.Customer.CompanyName, site.CreatedAt);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var site = await _context.Sites
            .Include(s => s.ConcreteRequests)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (site is null) return false;

        if (site.ConcreteRequests.Any())
            throw new ConflictException("Bu şantiyeye bağlı beton talepleri bulunduğundan silinemez.");

        _context.Sites.Remove(site);
        await _context.SaveChangesAsync();
        return true;
    }
}
