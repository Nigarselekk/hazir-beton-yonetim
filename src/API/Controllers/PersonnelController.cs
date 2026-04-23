using HazirBeton.API.Authorization;
using HazirBeton.Application.Features.Personnel;
using HazirBeton.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace HazirBeton.API.Controllers;

[ApiController]
[Route("api/personnel")]
public class PersonnelController : ControllerBase
{
    private readonly IPersonnelService _personnel;

    public PersonnelController(IPersonnelService personnel)
    {
        _personnel = personnel;
    }

    [HttpGet]
    [RequirePermission(Permission.PersonnelRead)]
    public async Task<IActionResult> GetAll([FromQuery] bool? isActive = null) =>
        Ok(await _personnel.GetAllAsync(isActive));

    [HttpGet("{id:guid}")]
    [RequirePermission(Permission.PersonnelRead)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _personnel.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [RequirePermission(Permission.PersonnelWrite)]
    public async Task<IActionResult> Create(CreatePersonnelRequest request)
    {
        try
        {
            var created = await _personnel.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [RequirePermission(Permission.PersonnelWrite)]
    public async Task<IActionResult> Update(Guid id, UpdatePersonnelRequest request)
    {
        try
        {
            var result = await _personnel.UpdateAsync(id, request);
            return result is null ? NotFound() : Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    [RequirePermission(Permission.PersonnelWrite)]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var result = await _personnel.DeactivateAsync(id);
        return result ? NoContent() : NotFound();
    }

    [HttpPost("{id:guid}/reactivate")]
    [RequirePermission(Permission.PersonnelWrite)]
    public async Task<IActionResult> Reactivate(Guid id)
    {
        var result = await _personnel.ReactivateAsync(id);
        return result ? NoContent() : NotFound();
    }
}
