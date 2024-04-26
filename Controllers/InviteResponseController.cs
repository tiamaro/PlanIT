using Microsoft.AspNetCore.Mvc;
using PlanIT.API.Services.Interfaces;


namespace PlanIT.API.Controllers;


// The InviteResponseController handles the confirmation of invitations sent to users.
// It leverages services to authenticate invitation tokens and confirm invitations based on validated claims.
// This controller includes logging to track the flow and results of operations, ensuring
// traceability of invite confirmations and related errors.


[Route("api/v1/[controller]")]
[ApiController]
public class InviteResponseController : ControllerBase
{
    private readonly ILogger<InviteResponseController> _logger;
    private readonly IInviteService _service;
    private readonly IEmailAuth _emailAuth;

    public InviteResponseController(ILogger<InviteResponseController> logger,
                                    IInviteService service,
                                    IEmailAuth emailAuth)
    {
        _logger = logger;
        _service = service;
        _emailAuth = emailAuth;
    }


    [HttpGet("confirm-invite")]
    public async Task<IActionResult> ConfirmInviteAsync(string token)
    {
        // Check if the token is not empty or null.
        if (string.IsNullOrWhiteSpace(token))
        {
            _logger.LogWarning("Empty or null token received.");
            return BadRequest("Token is required.");
        }


        // Validate the token and extract inviteId and eventId from it.
        var (inviteId, eventId) = _emailAuth.ValidateAndExtractClaims(token);


        // Attempt to confirm the invite using the extracted information.
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