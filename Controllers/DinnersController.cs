using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanIT.API.Extensions;
using PlanIT.API.Middleware;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Services.Interfaces;

namespace PlanIT.API.Controllers;

// DinnersController - API Controller for dinner management:
// - The controller handles all requests related to dinners, including registration,
//   updating, deletion, and retrieval of dinner information. It receives an instance of IDinnerService
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
[ServiceFilter(typeof(HandleExceptionFilter))]
public class DinnersController : ControllerBase
{
    private readonly ILogger<DinnersController> _logger;
    private readonly IDinnerService _dinnerService;

    public DinnersController(ILogger<DinnersController> logger, 
        IDinnerService dinnerService)
    {
        _logger = logger;
        _dinnerService = dinnerService;
    }


    // POST /api/v1/Dinner/register
    [HttpPost("register", Name = "AddDinner")]
    public async Task<ActionResult<DinnerDTO>> AddDinnerAsync(DinnerDTO newDinnerDTO)
    {
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        if (!ModelState.IsValid)
        {
            _logger.LogError("Invalid model state in AddDinnerAsync");
            return BadRequest(ModelState);
        }

        // Register dinner
        var addedDinner = await _dinnerService.CreateAsync(userId, newDinnerDTO);

        // Check if dinnerregistration is successfull
        return addedDinner != null
            ? Ok(addedDinner)
            : BadRequest("Failed to register new dinner");
    }


    // GET: api/v1/WeeklyDinnerPlan
    [HttpGet("weekly/{startDate}/{endDate}", Name = "GetWeeklyDinners")]
    public async Task<IActionResult> GetWeeklyDinnerPlan(DateOnly startDate, DateOnly endDate)
    {

        var userId = WebAppExtensions.GetValidUserId(HttpContext);
        var weeklyPlan = await _dinnerService.GetWeeklyDinnerPlanAsync(userId, startDate, endDate);

        return weeklyPlan != null
             ? Ok(weeklyPlan)
             : NotFound($"No weekly dinner plan found from {startDate} to {endDate}.");
    }



    [HttpGet(Name = "GetDinners")]
    public async Task<ActionResult<ICollection<DinnerDTO>>> GetDinnersAsync(int pageNr, int pageSize)
    {
        var userId = WebAppExtensions.GetValidUserId(HttpContext);
        var allDinners = await _dinnerService.GetAllAsync(userId, pageNr, pageSize);

        return allDinners != null
            ? Ok(allDinners)
            : NotFound("No registered dinners found.");
    }



    // GET /api/v1/Dinner/1
    [HttpGet("{dinnerId}", Name = "GetDinnerById")]
    public async Task<ActionResult<DinnerDTO>> GetDinnerByIdAsync(int dinnerId)
    {
        var userId = WebAppExtensions.GetValidUserId(HttpContext);
        var exsistingDinner = await _dinnerService.GetByIdAsync(userId, dinnerId);

        return exsistingDinner != null
            ? Ok(exsistingDinner)
            : NotFound("Dinner not found");
    }


   
    // PUT /api/v1/Dinner/4
    [HttpPut("{dinnerId}", Name = "UpdateDinner")]
    public async Task<ActionResult<DinnerDTO>> UpdateDinnerAsync(int dinnerId, DinnerDTO updatedDinnerDTO)
    {
        var userId = WebAppExtensions.GetValidUserId(HttpContext);
        var updatedDinnerResult = await _dinnerService.UpdateAsync(userId, dinnerId, updatedDinnerDTO);

        return updatedDinnerResult != null
            ? Ok(updatedDinnerResult)
            : NotFound($"Unable to update dinner with ID {dinnerId} or the dinner does not belong to the user");
    }


    // DELETE /api/v1/Dinner/2
    [HttpDelete("{dinnerId}", Name = "DeleteDinner")]
    public async Task<ActionResult<DinnerDTO>> DeleteDinnerAsync(int dinnerId)
    {
        var userId = WebAppExtensions.GetValidUserId(HttpContext);
        var deletedDinnerResult = await _dinnerService.DeleteAsync(userId, dinnerId);

        return deletedDinnerResult != null
            ? Ok(deletedDinnerResult)
            : BadRequest($"Unable to delete dinner with ID {dinnerId} or the dinner does not belong to the user");
    }
}