using Microsoft.AspNetCore.Mvc;
using PlanIT.API.Services.Interfaces;


namespace PlanIT.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class InviteResponseController : ControllerBase
{
    private readonly ILogger<InviteResponseController> _logger;
    private readonly IInviteService _service;
    private readonly IJWTEmailAuth _emailAuth;

    public InviteResponseController(ILogger<InviteResponseController> logger,
                                    IInviteService service,
                                    IJWTEmailAuth emailAuth)
    {
        _logger = logger;
        _service = service;
        _emailAuth = emailAuth;
    }


    [HttpGet("confirm-invite")]
    public async Task<IActionResult> ConfirmInviteAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            _logger.LogWarning("Empty or null token received.");
            return BadRequest("Token is required.");
        }
       
        var (inviteId, eventId) = _emailAuth.ValidateAndExtractClaims(token);

        var result = await _service.ConfirmInvite(inviteId, eventId);
        if (result)
        {
            _logger.LogInformation($"Invite confirmed for inviteId {inviteId} and eventId {eventId}.");
            return Ok("Invite confirmed");
        }
        else
        {
            _logger.LogWarning($"Unable to confirm invite for inviteId {inviteId} and eventId {eventId}.");
            return BadRequest("Unable to confirm invite");
        }
        
    }
}