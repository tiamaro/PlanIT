using Microsoft.AspNetCore.Mvc;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Services;
using PlanIT.API.Services.Interfaces;
using PlanIT.API.Utilities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PlanIT.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]

public class InviteResponseController : ControllerBase
{
    private readonly ILogger<InviteResponseController> _logger;
    private readonly IInviteService _service;
    private readonly IAuthService _authService;

    public InviteResponseController(ILogger<InviteResponseController> logger,
        IInviteService service,
        IAuthService authService)
    {
        _logger = logger;
        _service = service;
        _authService = authService;
    }


    //[HttpGet("confirm-invite")]
    //public async Task<IActionResult> ConfirmInviteAsync(string token)
    //{
    //    //try
    //    //{
    //    //    //    // method to decode token
    //    //    //    //var claims = _authService.Decode(token);
    //    //    //    int inviteId = int.Parse(claims["inviteId"]);  // Ensure the claim keys match those set during token creation
    //    //    //    int eventId = int.Parse(claims["eventId"]);

    //    //    var result = await _service.ConfirmInvite(inviteId, eventId);
    //    //    if (result)
    //    //    {
    //    //        return Ok("invite confirmed");
    //    //    }

    //    //    else
    //    //    {
    //    //        return BadRequest("Unableto confirm invite");
    //    //    }
    //    //}

    //    //catch
    //    //{
    //    //    return BadRequest("Invalid or expired token.");

    //    //}


    //    //}
}



