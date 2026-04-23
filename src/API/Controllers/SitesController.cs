using HazirBeton.API.Authorization;
using HazirBeton.Application.Exceptions;
using HazirBeton.Application.Features.Sites;
using HazirBeton.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace HazirBeton.API.Controllers;

[ApiController]
[Route("api/sites")]
public class SitesController : ControllerBase
{
    private readonly ISiteService _sites;

    public SitesController(ISiteService sites)
    {
        _sites = sites;
    }

    [HttpGet]
    [RequirePermission(Permission.CustomersRead)]
    public async Task<IActionResult> GetAll([FromQuery] Guid? customerId = null) =>
        Ok(await _sites.GetAllAsync(customerId));

    [HttpGet("{id:guid}")]
    [RequirePermission(Permission.CustomersRead)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _sites.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [RequirePermission(Permission.CustomersWrite)]
    public async Task<IActionResult> Create(CreateSiteRequest request)
    {
        try
        {
            var created = await _sites.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [RequirePermission(Permission.CustomersWrite)]
    public async Task<IActionResult> Update(Guid id, UpdateSiteRequest request)
    {
        try
        {
            var result = await _sites.UpdateAsync(id, request);
            return result is null ? NotFound() : Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission(Permission.CustomersWrite)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var deleted = await _sites.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
        catch (ConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
