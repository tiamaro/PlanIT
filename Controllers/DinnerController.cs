using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlanIT.API.Extensions;
using PlanIT.API.Middleware;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Services;
using PlanIT.API.Services.Interfaces;

namespace PlanIT.API.Controllers;


[Authorize(Policy = "Bearer")]
[ApiController]
[Route("api/v1/[controller]")]
[ServiceFilter(typeof(HandleExceptionFilter))]
public class DinnerController : ControllerBase
{
    private readonly ILogger<DinnerController> _logger;
    private readonly IService<DinnerDTO> _dinnerService;

    public DinnerController(ILogger<DinnerController> logger, IService<DinnerDTO> dinnerService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dinnerService = dinnerService ?? throw new ArgumentNullException(nameof(dinnerService));
    }

    [HttpPost("register", Name = "AddEvent")]
    public async Task<ActionResult<DinnerDTO>> AddDinnerAsync(DinnerDTO newDinnerDTO)
    {
        // Sjekk om modelltilstanden er gyldig etter modellbinding og validering
        if (!ModelState.IsValid)
        {
            _logger.LogError("Invalid model state in AddDinnerAsync");
            return BadRequest(ModelState);
        }

        // Registrer arrangementet
        var addedEvent = await _dinnerService.CreateAsync(newDinnerDTO);

        // Sjekk om arrangementsregistreringen var vellykket
        return addedEvent != null
            ? Ok(addedEvent)
            : BadRequest("Failed to register new dinner");
    }

    [HttpGet(Name = "GetDinners")]
    public async Task<ActionResult<ICollection<DinnerDTO>>> GetDinnersAsync(int pageNr, int pageSize)
    {
        var allDinners = await _dinnerService.GetAllAsync(pageNr, pageSize);

        return allDinners != null
            ? Ok(allDinners)
            : NotFound("No registered dinners found.");
    }


    [HttpGet("{dinnerId}", Name = "GetDinnerById")]
    public async Task<ActionResult<DinnerDTO>> GetDinnerByIdAsync(int dinnerId)
    {
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        var exsistingDinner = await _dinnerService.GetByIdAsync(userId, dinnerId);

        return exsistingDinner != null
            ? Ok(exsistingDinner)
            : NotFound("Dinner not found");

    }



    [HttpPut("{dinnerId}", Name = "UpdateEvent")]
    public async Task<ActionResult<DinnerDTO>> UpdateDinnerAsync(int dinnerId, DinnerDTO updatedDinnerDTO)
    {
        var userId = WebAppExtensions.GetValidUserId(HttpContext);


        var updatedEventResult = await _dinnerService.UpdateAsync(userId, dinnerId, updatedDinnerDTO);

        // Returnerer oppdatert arrangementdata, eller en feilmelding hvis oppdateringen mislykkes
        return updatedEventResult != null
            ? Ok(updatedEventResult)
            : NotFound("Unable to update the event or the event does not belong to the user");

    }

    [HttpDelete("{eventId}", Name = "DeleteEvent")]
    public async Task<ActionResult<DinnerDTO>> DeleteDinnerAsync(int dinnerId)
    {
        try
        {
            var deletedDinner = await _dinnerService.DeleteAsync(dinnerId);
            if (deletedDinner == null)
            {
                return NotFound();
            }
            return Ok(deletedDinner);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete dinner with ID {id}", dinnerId);
            return StatusCode(500, "An error occurred while deleting dinner");
        }
    }
}
