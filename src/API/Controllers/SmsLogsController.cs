using HazirBeton.Application.Exceptions;
using HazirBeton.Application.Features.Sms;
using HazirBeton.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HazirBeton.API.Controllers;

[ApiController]
[Route("api/sms-logs")]
[Authorize(Roles = nameof(UserRole.HeadManager))]
public class SmsLogsController : ControllerBase
{
    private readonly ISmsRetryService _retryService;

    public SmsLogsController(ISmsRetryService retryService)
    {
        _retryService = retryService;
    }

    [HttpPost("{id:guid}/retry")]
    public async Task<IActionResult> Retry(Guid id)
    {
        try
        {
            await _retryService.RetryAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
