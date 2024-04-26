using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanIT.API.Extensions;
using PlanIT.API.Middleware;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Services.Interfaces;

namespace PlanIT.API.Controllers;

// DinnerController - API Controller for dinner planning management:
// - The controller handles all requests related to dinner data, including registration,
//   updating, deletion, and retrieval of dinner information. It receives an instance of IService
//   as part of the constructor to perform operations related to dinners.
//
// Policy:
// - "Bearer": Requires that all calls to this controller are authenticated with a valid JWT token
//   that meets the requirements defined in the "Bearer" authentication policy. This ensures that only
//   authenticated users can access the endpoints defined in this controller.
//
// HandleExceptionFilter:
// - This filter is attached to the controller to catch and handle exceptions in a centralized manner.
//
// Requests starting with "api/v1/Dinners" will be routed to methods defined in this controller.

[Authorize(Policy = "Bearer")]
[ApiController]
[Route("api/v1/[controller]")]
[ServiceFilter(typeof(HandleExceptionFilter))] // Uses HandleExceptionFilter to handle exceptions
public class DinnersController : ControllerBase
{
    private readonly ILogger<DinnersController> _logger;
    private readonly IService<DinnerDTO> _dinnerService;

    public DinnersController(ILogger<DinnersController> logger, 
        IService<DinnerDTO> dinnerService)
    {
        _logger = logger;
        _dinnerService = dinnerService;
    }


    // Registers a new dinner
    // POST /api/v1/Dinners/register
    [HttpPost("register", Name = "AddDinner")]
    public async Task<ActionResult<DinnerDTO>> AddDinnerAsync(DinnerDTO newDinnerDTO)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);


        // Checks if the model state is valid after model binding and validation
        if (!ModelState.IsValid)
        {
            _logger.LogError("Invalid model state in AddDinnerAsync");
            return BadRequest(ModelState);
        }

       
        var addedDinner = await _dinnerService.CreateAsync(userId, newDinnerDTO);

        // Returns new dinner, or an error message if registration fails
        return addedDinner != null
            ? Ok(addedDinner)
            : BadRequest("Failed to register new dinner");
    }


    // Retrieves a paginated list of dinners.
    // GET: /api/v1/Dinners?pageNr=1&pageSize=10
    [HttpGet(Name = "GetDinners")]
    public async Task<ActionResult<ICollection<DinnerDTO>>> GetDinnersAsync(int pageNr, int pageSize)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);


        var allDinners = await _dinnerService.GetAllAsync(userId,pageNr, pageSize);

        // Returns list of dinners, or an error message if not found
        return allDinners != null
            ? Ok(allDinners)
            : NotFound("No registered dinners found.");
    }



    // Retrieves a specific dinner by its ID.
    // GET /api/v1/Dinners/{id}
    [HttpGet("{dinnerId}", Name = "GetDinnerById")]
    public async Task<ActionResult<DinnerDTO>> GetDinnerByIdAsync(int dinnerId)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        
        var exsistingDinner = await _dinnerService.GetByIdAsync(userId, dinnerId);

        // Returns dinner or an error message if not found
        return exsistingDinner != null
            ? Ok(exsistingDinner)
            : NotFound("Dinner not found");
    }


    // Updates a dinner based on the provided ID.
    // PUT /api/v1/Dinners/{id}
    [HttpPut("{dinnerId}", Name = "UpdateDinner")]
    public async Task<ActionResult<DinnerDTO>> UpdateDinnerAsync(int dinnerId, DinnerDTO updatedDinnerDTO)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        
        var updatedDinnerResult = await _dinnerService.UpdateAsync(userId, dinnerId, updatedDinnerDTO);

        // Returns updated dinner or an error message if not found
        return updatedDinnerResult != null
            ? Ok(updatedDinnerResult)
            : NotFound("Unable to update dinner or the dinner does not belong to the user");
    }


    // Deletes a dinner based on the provided ID.
    // DELETE /api/v1/Dinners/{id}
    [HttpDelete("{dinnerId}", Name = "DeleteDinner")]
    public async Task<ActionResult<DinnerDTO>> DeleteDinnerAsync(int dinnerId)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        
        var deletedDinnerResult = await _dinnerService.DeleteAsync(userId, dinnerId);


        // Returns deleted dinner or an error message if deletion fails
        return deletedDinnerResult != null
            ? Ok(deletedDinnerResult)
            : NotFound("Unable to delete dinner or the dinner does not belong to the user");

    }
}