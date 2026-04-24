using HazirBeton.API.Authorization;
using HazirBeton.Application.Common;
using HazirBeton.Application.Exceptions;
using HazirBeton.Application.Features.ConcreteRequests;
using HazirBeton.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace HazirBeton.API.Controllers;

[ApiController]
[Route("api/concrete-requests")]
public class ConcreteRequestsController : ControllerBase
{
    private readonly IConcreteRequestService _concreteRequests;
    private readonly ICurrentUserService _currentUser;

    public ConcreteRequestsController(
        IConcreteRequestService concreteRequests,
        ICurrentUserService currentUser)
    {
        _concreteRequests = concreteRequests;
        _currentUser = currentUser;
    }

    [HttpGet]
    [RequirePermission(Permission.OrdersRead)]
    public async Task<IActionResult> GetList([FromQuery] ConcreteRequestFilterRequest filter) =>
        Ok(await _concreteRequests.GetListAsync(filter));

    [HttpGet("{id:guid}")]
    [RequirePermission(Permission.OrdersRead)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _concreteRequests.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [RequirePermission(Permission.OrdersCreate)]
    public async Task<IActionResult> Create(CreateConcreteRequestRequest request)
    {
        try
        {
            var created = await _concreteRequests.CreateAsync(request, _currentUser.UserId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}/approve")]
    [RequirePermission(Permission.OrdersApprove)]
    public async Task<IActionResult> Approve(Guid id, ApproveConcreteRequestRequest request)
    {
        try
        {
            var result = await _concreteRequests.ApproveAsync(id, request, _currentUser.UserId);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/vehicles")]
    [RequirePermission(Permission.OrdersAssignVehicle)]
    public async Task<IActionResult> AssignVehicle(Guid id, AssignVehicleRequest request)
    {
        try
        {
            var result = await _concreteRequests.AssignVehicleAsync(id, request, _currentUser.UserId);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}/vehicles/{vehicleId:guid}")]
    [RequirePermission(Permission.OrdersAssignVehicle)]
    public async Task<IActionResult> RemoveVehicle(Guid id, Guid vehicleId)
    {
        try
        {
            await _concreteRequests.RemoveVehicleAsync(id, vehicleId);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}/deliver")]
    [RequirePermission(Permission.OrdersDeliveryEntry)]
    public async Task<IActionResult> Deliver(Guid id, DeliveryEntryRequest request)
    {
        try
        {
            var result = await _concreteRequests.DeliverAsync(id, request, _currentUser.UserId);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}/cancel")]
    [RequirePermission(Permission.OrdersCancel)]
    public async Task<IActionResult> Cancel(Guid id, CancelConcreteRequestRequest request)
    {
        try
        {
            var result = await _concreteRequests.CancelAsync(id, request, _currentUser.UserId);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
