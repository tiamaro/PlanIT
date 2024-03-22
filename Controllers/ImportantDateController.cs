using Microsoft.AspNetCore.Mvc;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Services.Interfaces;

namespace PlanIT.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ImportantDateController : ControllerBase
{
    private readonly IService<ImportantDateDTO> _dateService;
    private readonly ILogger<ImportantDateController> _logger;

    public ImportantDateController(IService<ImportantDateDTO> dateService,
        ILogger<ImportantDateController> logger)
    {
        _dateService = dateService;
        _logger = logger;
    }

    [HttpPost("register", Name = "AddImportantDate")]
    public async Task<ActionResult<ImportantDateDTO>> AddImportantDateAsync(ImportantDateDTO newDateDTO)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state in AddImportantDateAsync");
                return BadRequest(ModelState);

            }

            var addedImportantDate = await _dateService.CreateAsync(newDateDTO);
            return addedImportantDate != null ? Ok(addedImportantDate) : BadRequest("Failed to register new ImportantDate");

        }

        catch (Exception ex)
        {
            _logger.LogError($"An unknown error occured: {ex.Message}", ex);
            return StatusCode(500, "An unknown error occured, please try again later");


        }
    }

    [HttpGet(Name = "GetImportantDates")]
    public async Task<ActionResult<IEnumerable<ImportantDateDTO>>> GetImportantDatesAsync(int pageNr, int pageSize)
    {
        var allImportantDates = await _dateService.GetAllAsync(pageNr, pageSize);
        return allImportantDates != null ? Ok(allImportantDates) : NotFound("No registrated ImportantDates found");

    }


    [HttpGet("{importantDateId}", Name = "GetImportantDatesById")]
    public async Task<ActionResult<ImportantDateDTO>> GetImportantDatesByIdAsync(int importantDateId)
    {
        var exsistingImportantDate = await _dateService.GetByIdAsync(importantDateId);
        return exsistingImportantDate != null ? Ok(importantDateId) : NotFound("ImportantDate not found");

    }

    [HttpPut("{importantDateId}", Name = "UpdateImportantDate")]
    public async Task<ActionResult<ImportantDateDTO>> updateImportantDateAsync(int importantDateId, ImportantDateDTO updatedImportantDateDTO)
    {
        var exsistingImportantDate = await _dateService.GetByIdAsync(importantDateId);

        if (updatedImportantDateDTO == null) return NotFound("ImportantDate not found");


        try
        {
            var updatedImportantDate = await _dateService.UpdateAsync(importantDateId, updatedImportantDateDTO);
            return updatedImportantDate != null ? Ok(updatedImportantDate) : NotFound("Unable to update the ImportantDate");
        }

        catch (Exception ex)
        {
            _logger.LogError($"An unknown error occured: {ex.Message}", ex);
            return StatusCode(500, "An unknown error occured, please try again later");

        }         

    }


    [HttpDelete("{importantDateId}", Name = "DeleteImportantDate")]
    public async Task<ActionResult<ImportantDateDTO>> DeleteImportantDateAsync(int importantDateId)
    {
        var exsistingImportantDate = await _dateService.GetByIdAsync(importantDateId);
        if (exsistingImportantDate == null) return NotFound("ImportantDate not found");

        try
        {
            var deletedImportantDate = await _dateService.DeleteAsync(importantDateId);
            return deletedImportantDate != null ? Ok(deletedImportantDate) : BadRequest("Unable to delete ImportantDate");

        }

        catch (Exception ex)
        {
            _logger.LogError($"An unknown error occured: {ex.Message}", ex);
            return StatusCode(500, "An unknown error occured, please try again later");

        }


    }




}
