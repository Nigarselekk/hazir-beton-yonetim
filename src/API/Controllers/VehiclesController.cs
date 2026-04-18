using HazirBeton.Application.Features.Vehicles;
using HazirBeton.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace HazirBeton.API.Controllers;

[ApiController]
[Route("api/vehicles")]
public class VehiclesController : ControllerBase
{
    private readonly IVehicleService _vehicles;

    public VehiclesController(IVehicleService vehicles)
    {
        _vehicles = vehicles;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] VehicleStatus? status = null) =>
        Ok(await _vehicles.GetAllAsync(status));

    [HttpGet("maintenance-alerts")]
    public async Task<IActionResult> GetMaintenanceAlerts([FromQuery] int? days = null) =>
        Ok(await _vehicles.GetMaintenanceAlertsAsync(days));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _vehicles.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateVehicleRequest request)
    {
        try
        {
            var created = await _vehicles.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateVehicleRequest request)
    {
        try
        {
            var result = await _vehicles.UpdateAsync(id, request);
            return result is null ? NotFound() : Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var result = await _vehicles.DeactivateAsync(id);
        return result ? NoContent() : NotFound();
    }
}
