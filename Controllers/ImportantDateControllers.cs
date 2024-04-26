using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PlanIT.API.Extensions;
using PlanIT.API.Middleware;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Services.Interfaces;

namespace PlanIT.API.Controllers;

// ImportantDateController - API Controller for managing important dates:
// - The controller handles all requests related to important dates, including registration,
//   updating, deletion, and retrieval of date information. It receives an instance of IService
//   as part of the constructor to perform operations related to important dates.
//
// Policy:
// - "Bearer": Requires that all calls to this controller are authenticated with a valid JWT token
//   that meets the requirements defined in the "Bearer" authentication policy. This ensures that only
//   authenticated users can access the endpoints defined in this controller.
//
// HandleExceptionFilter:
// - This filter is attached to the controller to catch and handle exceptions in a centralized manner.
//
// Requests starting with "api/v1/ImportantDates" will be routed to methods defined in this controller.



[Authorize(Policy = "Bearer")]
[Route("api/v1/[controller]")]
[ApiController]
[ServiceFilter(typeof(HandleExceptionFilter))] // Uses HandleExceptionFilter to handle exceptions
public class ImportantDateControllers : ControllerBase
{
    private readonly IService<ImportantDateDTO> _dateService;
    private readonly ILogger<ImportantDateControllers> _logger;

    public ImportantDateControllers(IService<ImportantDateDTO> dateService,
        ILogger<ImportantDateControllers> logger)
    {
        _dateService = dateService;
        _logger = logger;
    }


    // Registers a new important date
    // POST /api/v1/ImportantDates/register
    [HttpPost("register", Name = "AddImportantDate")]
    public async Task<ActionResult<ImportantDateDTO>> AddImportantDateAsync(ImportantDateDTO newImportantDateDTO)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        // Checks if the model state is valid after model binding and validation
        if (!ModelState.IsValid)
        {
            _logger.LogError("Invalid model state in AddImportantDateAsync");
            return BadRequest(ModelState);
        }

        
        var addedImportantDate = await _dateService.CreateAsync(userId, newImportantDateDTO);

        // Returns new important date, or an error message if registration fails
        return addedImportantDate != null
            ? Ok(addedImportantDate)
            : BadRequest("Failed to register new ImportantDate");

    }



    // Retrieves a paginated list of important dates.
    // GET: /api/v1/ImportantDates?pageNr=1&pageSize=10
    [HttpGet(Name = "GetImportantDates")]
    public async Task<ActionResult<IEnumerable<ImportantDateDTO>>> GetImportantDatesAsync(int pageNr, int pageSize)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);


        var allImportantDates = await _dateService.GetAllAsync(userId, pageNr, pageSize);


        // Returns list of important dates, or an error message if not found
        return allImportantDates != null
            ? Ok(allImportantDates)
            : NotFound("No registered ImportantDates found.");

    }


    // Retrieves a specific important date by its ID.
    // GET /api/v1/ImportantDates/{id}
    [HttpGet("{importantDateId}", Name = "GetImportantDatesById")]
    public async Task<ActionResult<ImportantDateDTO>> GetImportantDatesByIdAsync(int importantDateId)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        
        var exsistingImportantDate = await _dateService.GetByIdAsync(userId, importantDateId);


        // Returns important date, or an error message if not found
        return exsistingImportantDate != null
            ? Ok(exsistingImportantDate)
            : NotFound("ImportantDate not found");
    }



    // Updates an important date based on the provided ID.
    // PUT /api/v1/ImportantDates/{id}
    [HttpPut("{importantDateId}", Name = "UpdateImportantDate")]
    public async Task<ActionResult<ImportantDateDTO>> updateImportantDateAsync(int importantDateId, ImportantDateDTO updatedImportantDateDTO)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        
        var updatedImportantDate = await _dateService.UpdateAsync(userId, importantDateId, updatedImportantDateDTO);

        // Returns updated important date, or an error message if update fails
        return updatedImportantDate != null
            ? Ok(updatedImportantDate)
            : NotFound("Unable to update the important date item or the item does not belong to the user");

    }


    // Deletes an important date based on the provided ID.
    // DELETE /api/v1/ImportantDates/{id}
    [HttpDelete("{importantDateId}", Name = "DeleteImportantDate")]
    public async Task<ActionResult<ImportantDateDTO>> DeleteImportantDateAsync(int importantDateId)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

       
        var deletedImportantDateResult = await _dateService.DeleteAsync(userId, importantDateId);

        // Returns deleted important date, or an error message if deletion fails
        return deletedImportantDateResult != null
            ? Ok(deletedImportantDateResult)
            : NotFound("Unable to delete important date item or the item does not belong to the user");

    }

}