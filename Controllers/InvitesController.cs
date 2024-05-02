using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanIT.API.Extensions;
using PlanIT.API.Middleware;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Services.Interfaces;

namespace PlanIT.API.Controllers;


// InvitesController - API Controller for invitation management:
// - The controller handles all requests related to invitations, including registration,
//   updating, deletion, and retrieval of invitation information. It receives an instance of IService
//   as part of the constructor to perform operations related to invitations.
//
// Policy:
// - "Bearer": Requires that all calls to this controller are authenticated with a valid JWT token
//   that meets the requirements defined in the "Bearer" authentication policy. This ensures that only
//   authenticated users can access the endpoints defined in this controller.
//
// HandleExceptionFilter:
// - This filter is attached to the controller to catch and handle exceptions in a centralized manner.
//
// Requests starting with "api/v1/Invites" will be routed to methods defined in this controller.

[Authorize(Policy = "Bearer")]
[Route("api/v1/[controller]")]
[ApiController]
[ServiceFilter(typeof(HandleExceptionFilter))]  // Uses HandleExceptionFilter to handle exceptions

public class InvitesController : ControllerBase
{
    private readonly IInviteService _inviteService;
    private readonly ILogger<InvitesController> _logger;

    public InvitesController(IInviteService inviteService, 
        ILogger<InvitesController> logger)
    {
        _inviteService = inviteService;
        _logger = logger;
    }


    // Registers a new invite
    // POST /api/v1/Invites/register
    [HttpPost("register", Name = "AddInvites")]
    public async Task<ActionResult<InviteDTO>> AddInviteAsync(InviteDTO newInviteDTO)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        // Checks if the model state is valid after model binding and validation
        if (!ModelState.IsValid)
        {
            _logger.LogError("Invalid model state in AddInviteAsync");
            return BadRequest(ModelState);
        }

       
        var addedInvite = await _inviteService.CreateAsync(userId, newInviteDTO);

        // Returns new invite, or an error message if registration fails
        return addedInvite != null
            ? Ok(addedInvite)
            : BadRequest("Failed to register new invite");
    }


    // Retrieves a paginated list of invites.
    // GET: /api/v1/Invites?pageNr=1&pageSize=10
    [HttpGet(Name = "GetInvites")]
    public async Task<ActionResult<IEnumerable<InviteDTO>>> GetInvitesAsync(int pageNr, int pageSize)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        var invites = await _inviteService.GetAllAsync(userId, pageNr, pageSize);


        // Returns list of invites, or an error message if not found
        return invites != null
            ? Ok(invites)
            : NotFound("No registered invites found.");
    }


    // Retrieves a specific invite by its ID.
    // GET /api/v1/Invites/{id}
    [HttpGet("{inviteId}", Name = "GetInvitesById")]
    public async Task<ActionResult<InviteDTO>> GetInvitesByIdASync(int inviteId)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        
        var existingInvite = await _inviteService.GetByIdAsync(userId, inviteId);


        // Returns invite, or an error message if not found
        return existingInvite != null
            ? Ok(existingInvite)
            : NotFound("Invite not found");
    }



    // Updates an invite based on the provided ID.
    // PUT /api/v1/Invites/{id}
    [HttpPut("{inviteId}", Name = "UpdateInvite")]
    public async Task<ActionResult<InviteDTO>> UpdateInviteAsync(int inviteId, InviteDTO updatedInviteDTO)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        
        var updatedInviteResult = await _inviteService.UpdateAsync(userId, inviteId, updatedInviteDTO);


        // Returns updated invite, or an error message if update fails
        return updatedInviteResult != null
            ? Ok(updatedInviteResult)
            : NotFound("Unable to update the invite or the invite does not belong to the user");
    }


    // Deletes an invite based on the provided ID.
    // DELETE /api/v1/Invites/2
    [HttpDelete("{inviteId}", Name = "DeleteInvite")]
    public async Task<ActionResult<InviteDTO>> DeleteInviteAsync(int inviteId)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        
        var deletedInviteResult = await _inviteService.DeleteAsync(userId, inviteId);


        // Returns deleted invite, or an error message if deletion fails
        return deletedInviteResult != null
            ? Ok(deletedInviteResult)
            : BadRequest("Unable to delete invite or the invite does not belong to the user");
    }
}
