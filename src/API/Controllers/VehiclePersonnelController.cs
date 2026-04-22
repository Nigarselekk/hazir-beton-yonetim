using HazirBeton.Application.Exceptions;
using HazirBeton.Application.Features.VehiclePersonnel;
using Microsoft.AspNetCore.Mvc;

namespace HazirBeton.API.Controllers;

[ApiController]
[Route("api/vehicles/{vehicleId:guid}/personnel")]
public class VehiclePersonnelController : ControllerBase
{
    private readonly IVehiclePersonnelService _service;

    public VehiclePersonnelController(IVehiclePersonnelService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetByVehicle(Guid vehicleId) =>
        Ok(await _service.GetByVehicleAsync(vehicleId));

    [HttpPost]
    public async Task<IActionResult> Assign(Guid vehicleId, AssignPersonnelRequest request)
    {
        try
        {
            var result = await _service.AssignAsync(vehicleId, request);
            return Created(string.Empty, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{personnelId:guid}")]
    public async Task<IActionResult> UpdateRole(Guid vehicleId, Guid personnelId, UpdateAssignmentTypeRequest request)
    {
        try
        {
            var result = await _service.UpdateRoleAsync(vehicleId, personnelId, request);
            return result is null ? NotFound() : Ok(result);
        }
        catch (ConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{personnelId:guid}")]
    public async Task<IActionResult> Remove(Guid vehicleId, Guid personnelId)
    {
        var result = await _service.RemoveAsync(vehicleId, personnelId);
        return result ? NoContent() : NotFound();
    }
}
