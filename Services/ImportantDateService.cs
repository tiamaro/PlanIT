using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Services.Interfaces;

namespace PlanIT.API.Services;

public class ImportantDateService : IService<ImportantDateDTO>
{
    private readonly IMapper<ImportantDate, ImportantDateDTO> _dateMapper;
    private readonly IRepository<ImportantDate> _dateRepository;
    private readonly ILogger<ImportantDateService> _logger;

    public ImportantDateService(IMapper<ImportantDate, ImportantDateDTO> dateMapper,
        IRepository<ImportantDate> dateRepository,
        ILogger<ImportantDateService> logger)
    {
        _dateMapper = dateMapper;
        _dateRepository = dateRepository;
        _logger = logger;
    }


    // Oppretter nytt ImportantDate
    public async Task<ImportantDateDTO?> CreateAsync(ImportantDateDTO newImportantDateDTO)
    {
        _logger.LogInformation("Starting to create a new ImportantDate.");

        var newImportantDate = _dateMapper.MapToModel(newImportantDateDTO);


        var addedImportantDate = await _dateRepository.AddAsync(newImportantDate);
        if (addedImportantDate == null)
        {
            _logger.LogWarning("Failed to create new ImportantDate.");
            throw new InvalidOperationException("The ImportantDate could not be created due to invalid data or state.");
        }

        _logger.LogInformation("New event created successfully with ID {ImportantDateId}.", addedImportantDate.Id);
        return _dateMapper.MapToDTO(addedImportantDate);


    }


    // Henter alle ImportantDates med paginering
    public async Task<ICollection<ImportantDateDTO>> GetAllAsync(int pageNr, int pageSize)
    {
        var importantDatesFromRepository = await _dateRepository.GetAllAsync(1, 10);
        var importantDateDTOs = importantDatesFromRepository.Select(dateEntity => _dateMapper.MapToDTO(dateEntity)).ToList();
        return importantDateDTOs;
    }


    // Henter ImportantDate basert på ID
    public async Task<ImportantDateDTO?> GetByIdAsync(int userIdFromToken, int importantDateId)
    {
        _logger.LogDebug("Attempting to retrieve event with ID {ImportantDateId} for user ID {UserId}.", importantDateId, userIdFromToken);


        var importantDateFromRepository = await _dateRepository.GetByIdAsync(importantDateId);
        if (importantDateFromRepository == null)
        {
            _logger.LogWarning("ImportantDate with ID {ImportantDate} not found.", importantDateId);
            throw new KeyNotFoundException($"ImportantDate with ID {importantDateId} not found.");
        }

        if (importantDateFromRepository.Id != userIdFromToken)
        {
            _logger.LogWarning("Unauthorized attempt to access ImportantDate with ID {ImportantDateId} by user ID {UserId}.", importantDateId, userIdFromToken);
            throw new UnauthorizedAccessException($"User ID {userIdFromToken} is not authorized to access ImportantDate ID {importantDateId}.");
        }

        _logger.LogInformation("Event with ID {ImportantDateId} retrieved successfully for user ID {UserId}.", importantDateId, userIdFromToken);
        return _dateMapper.MapToDTO(importantDateFromRepository);




    }

    // Oppdaterer ImportantDate
    public async Task<ImportantDateDTO?> UpdateAsync(int userIdFromToken, int importantDateId, ImportantDateDTO dateDTO)
    {
        _logger.LogDebug("Attempting to update ImportantDate with ID {ImportantDateId} for user ID {UserId}.", importantDateId, userIdFromToken);

        var exsistingImportantDate = await _dateRepository.GetByIdAsync(importantDateId);
        if (exsistingImportantDate == null)
        {
            _logger.LogWarning("ImportantDate with ID {ImportantDateId} not found.", importantDateId);
            throw new KeyNotFoundException($"ImportantDate with ID {importantDateId} not found.");
        }

        if (exsistingImportantDate.Id != userIdFromToken)
        {
            _logger.LogWarning("Unauthorized update attempt by User ID {UserId} on ImportantDate ID {ImportantDateId}", userIdFromToken, importantDateId);
            throw new UnauthorizedAccessException($"User ID {userIdFromToken} is not authorized to update ImportantDate ID {importantDateId}.");

        }

        var importantDateToUpdate = _dateMapper.MapToModel(dateDTO);
        importantDateToUpdate.Id = importantDateId;


        var updatedImportantDate = await _dateRepository.UpdateAsync(importantDateId, importantDateToUpdate);
        if (updatedImportantDate == null)
        {
            _logger.LogError("Failed to update ImportantDate with ID {ImportantDateId}", importantDateId);
            throw new InvalidOperationException($"Failed to update ImportantDate with ID {importantDateId}.");

        }

        _logger.LogInformation("Event with ID {ImportantDateId} updated successfully by User ID {UserId}.", importantDateId, userIdFromToken);
        return _dateMapper.MapToDTO(updatedImportantDate);

    }


    // Sletter ImportantDate
    public async Task<ImportantDateDTO?> DeleteAsync(int userIdFromToken, int importantDateId)
    {
        _logger.LogDebug("Attempting to delete event with ID {ImportantDateId} by user ID {UserId}.", userIdFromToken, userIdFromToken);

        var importantDateToDelete = await _dateRepository.GetByIdAsync(importantDateId);
        if (importantDateToDelete == null)
        {
            _logger.LogWarning("Attempt to delete a non-existent ImportantDate with ID {ImportantDateId}.", importantDateId);
            throw new KeyNotFoundException($"ImportantDate with ID {importantDateId} not found.");
        }

        if (importantDateToDelete.Id != userIdFromToken)
        {
            _logger.LogWarning("Unauthorized delete attempt by User ID {UserId} on ImportantDate ID {ImportantDateId}.", userIdFromToken, importantDateId);
            throw new UnauthorizedAccessException($"User ID {userIdFromToken} is not authorized to delete ImportantDate ID {importantDateId}.");

        }


        var deltedImportantDate = await _dateRepository.DeleteAsync(importantDateId);
        if (deltedImportantDate == null)
        {
            _logger.LogError("Failed to delete ImportantDate with ID {ImportantDateId}.", importantDateId);
            throw new InvalidOperationException("Deletion failed, could not complete operation on the database.");

        }


        _logger.LogInformation("Event with ID {ImportantDateId} deleted successfully by User ID {UserId}.", importantDateId, userIdFromToken);
        return _dateMapper.MapToDTO(importantDateToDelete);

    }
}
