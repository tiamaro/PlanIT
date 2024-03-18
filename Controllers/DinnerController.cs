using Microsoft.AspNetCore.Mvc;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Services.Interfaces;

namespace PlanIT.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class DinnerController : ControllerBase
{
    private readonly ILogger<DinnerController> _logger;
    private readonly IService<DinnerDTO> _dinnerService;

    public DinnerController(ILogger<DinnerController> logger, IService<DinnerDTO> dinnerService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dinnerService = dinnerService ?? throw new ArgumentNullException(nameof(dinnerService));
    }

    [HttpPost]
    public async Task<ActionResult<DinnerDTO>> CreateDinnerAsync(DinnerDTO newDinnerDto)
    {
        try
        {
            var createdDinner = await _dinnerService.CreateAsync(newDinnerDto);
            if (createdDinner == null)
            {
                return StatusCode(500, "Failed to create dinner");
            }

            return Ok(createdDinner);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create dinner");
            return StatusCode(500, "An error occurred while creating dinner");
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DinnerDTO>> GetDinnerByIdAsync(int id)
    {
        var dinner = await _dinnerService.GetByIdAsync(id);
        if (dinner == null)
        {
            return NotFound();
        }
        return Ok(dinner);
    }

    [HttpGet]
    public async Task<ActionResult<ICollection<DinnerDTO>>> GetDinnersAsync([FromQuery] int pageNr, [FromQuery] int pageSize)
    {
        if (pageNr <= 0 || pageSize <= 0)
        {
            return BadRequest("Invalid page number or page size");
        }

        var dinners = await _dinnerService.GetAllAsync(pageNr, pageSize);
        return Ok(dinners);
    }
    
    [HttpPut("{id:int}")]
    public async Task<ActionResult<DinnerDTO>> UpdateDinnerAsync(int id, DinnerDTO updatedDinnerDto)
    {
        try
        {
            var updatedDinner = await _dinnerService.UpdateAsync(id, updatedDinnerDto);
            if (updatedDinner == null)
            {
                return NotFound();
            }
            return Ok(updatedDinner);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update dinner with ID {id}", id);
            return StatusCode(500, "An error occurred while updating dinner");
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<DinnerDTO>> DeleteDinnerAsync(int id)
    {
        try
        {
            var deletedDinner = await _dinnerService.DeleteAsync(id);
            if (deletedDinner == null)
            {
                return NotFound();
            }
            return Ok(deletedDinner);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete dinner with ID {id}", id);
            return StatusCode(500, "An error occurred while deleting dinner");
        }
    }
}
