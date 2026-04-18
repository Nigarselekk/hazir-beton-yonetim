using HazirBeton.Application.Exceptions;
using HazirBeton.Application.Features.Customers;
using HazirBeton.Application.Features.Sites;
using Microsoft.AspNetCore.Mvc;

namespace HazirBeton.API.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customers;
    private readonly ISiteService _sites;

    public CustomersController(ICustomerService customers, ISiteService sites)
    {
        _customers = customers;
        _sites = sites;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _customers.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _customers.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("{id:guid}/sites")]
    public async Task<IActionResult> GetSites(Guid id) =>
        Ok(await _sites.GetAllAsync(customerId: id));

    [HttpPost]
    public async Task<IActionResult> Create(CreateCustomerRequest request)
    {
        try
        {
            var created = await _customers.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateCustomerRequest request)
    {
        try
        {
            var result = await _customers.UpdateAsync(id, request);
            return result is null ? NotFound() : Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var deleted = await _customers.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
        catch (ConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
