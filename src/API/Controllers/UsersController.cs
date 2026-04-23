using HazirBeton.Application.Exceptions;
using HazirBeton.Application.Features.Users;
using HazirBeton.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HazirBeton.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = nameof(UserRole.HeadManager))]
public class UsersController : ControllerBase
{
    private readonly IUserService _users;

    public UsersController(IUserService users)
    {
        _users = users;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _users.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _users.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserRequest request)
    {
        try
        {
            var created = await _users.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
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

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateUserRequest request)
    {
        try
        {
            var result = await _users.UpdateAsync(id, request);
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
        var result = await _users.DeactivateAsync(id);
        return result ? NoContent() : NotFound();
    }

    [HttpPost("{id:guid}/reactivate")]
    public async Task<IActionResult> Reactivate(Guid id)
    {
        var result = await _users.ReactivateAsync(id);
        return result ? NoContent() : NotFound();
    }

    [HttpGet("{id:guid}/permissions")]
    public async Task<IActionResult> GetPermissions(Guid id)
    {
        var user = await _users.GetByIdAsync(id);
        return user is null ? NotFound() : Ok(user.Permissions);
    }

    [HttpPut("{id:guid}/permissions")]
    public async Task<IActionResult> SetPermissions(Guid id, SetPermissionsRequest request)
    {
        try
        {
            await _users.SetPermissionsAsync(id, request.Permissions);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
