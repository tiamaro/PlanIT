using Microsoft.AspNetCore.Mvc;
using PlanIT.API.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace PlanIT.API.Controllers
{
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
            try
            {
                var principal = _emailAuth.DecodeToken(token);
                var inviteIdClaim = principal.FindFirst("inviteId")?.Value;
                var eventIdClaim = principal.FindFirst("eventId")?.Value;

                if (inviteIdClaim == null || eventIdClaim == null)
                {
                    _logger.LogWarning("Token claims 'inviteId' or 'eventId' not found.");
                    return BadRequest("Invalid token claims.");
                }

                int inviteId = int.Parse(inviteIdClaim);
                int eventId = int.Parse(eventIdClaim);

                var result = await _service.ConfirmInvite(inviteId, eventId);
                if (result)
                {
                    return Ok("Invite confirmed");
                }
                else
                {
                    return BadRequest("Unable to confirm invite");
                }
            }
            catch (SecurityTokenExpiredException ex)
            {
                _logger.LogError(ex, "Attempted to use an expired token.");
                return BadRequest("Token has expired.");
            }
            catch (SecurityTokenValidationException ex)
            {
                _logger.LogError(ex, "Token validation failed.");
                return BadRequest("Invalid token.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while confirming the invite.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}