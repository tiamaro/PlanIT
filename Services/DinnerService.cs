using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Services.Interfaces;

namespace PlanIT.API.Services;

public class DinnerService : IService<DinnerDTO>
{
    private readonly IMapper<Dinner, DinnerDTO> _dinnerMapper;
    private readonly IRepository<Dinner> _dinnerRepository;
    private readonly ILogger<DinnerService> _logger;

    public DinnerService(IMapper<Dinner,
        DinnerDTO> dinnerMapper,
        IRepository<Dinner> dinnerRepository,
        ILogger<DinnerService> logger)
    {
        _dinnerMapper = dinnerMapper;
        _dinnerRepository = dinnerRepository;
        _logger = logger;
    }

    // Creates a new dinner asynchronously.
    public async Task<DinnerDTO?> CreateAsync(DinnerDTO dinnerDTO)
    {
        var newDinner = _dinnerMapper.MapToModel(dinnerDTO);
        var addedDinner = await _dinnerRepository.AddAsync(newDinner);
        if (addedDinner == null)
        {
            _logger.LogWarning("Failed to create new Dinner.");
            throw new InvalidOperationException("The Dinner could not be created due to invalid data or state");
        }

        _logger.LogInformation("New Dinner created sucessfully with ID {DinnerId}.", addedDinner.Id);
        return _dinnerMapper.MapToDTO(addedDinner);
    }

    // Retrieves all dinners with pagination asynchronously.
    public async Task<ICollection<DinnerDTO>> GetAllAsync(int pageNr, int pageSize)
    {
        var dinnersFromRepository = await _dinnerRepository.GetAllAsync(1, 10);
        var dinnerDTOs = dinnersFromRepository.Select(dinnerEntity => _dinnerMapper.MapToDTO(dinnerEntity)).ToList();
        return dinnerDTOs;
    }

    // Retrieves a dinner by ID asynchronously.
    public async Task<DinnerDTO?> GetByIdAsync(int userIdFromToken, int dinnerId)
    {
        _logger.LogDebug("Attempting to retrieve Dinner with ID {EventId} for user ID {UserId}", dinnerId, userIdFromToken);

        var dinnerFromRepostiroy = await _dinnerRepository.GetByIdAsync(dinnerId);

        if (dinnerFromRepostiroy == null)
        {
            _logger.LogWarning("Unauthorized attempt to access Dinner with ID {DinnerId} by user ID {UserId}.", dinnerId, userIdFromToken);
            throw new UnauthorizedAccessException($"User ID {userIdFromToken} is not authorized to access dinner ID {dinnerId}.");
        }

        if (dinnerFromRepostiroy.UserId != userIdFromToken)
        {
            _logger.LogWarning("Unauthorized attempt to access event with ID {DinnerId} by user ID {UserId}.", dinnerId, userIdFromToken);
            throw new UnauthorizedAccessException($"User ID {userIdFromToken} is not authorized to access dinner ID {dinnerId}.");

        }

        _logger.LogInformation("Event with ID {DinnerId} retrieved successfully for user ID {UserId}.", dinnerId, userIdFromToken);
        return _dinnerMapper.MapToDTO(dinnerFromRepostiroy);


    }

    // Updates a dinner asynchronously.
    public async Task<DinnerDTO?> UpdateAsync(int userIdFromToken, int dinnerId, DinnerDTO dinnerDTO)
    {
        _logger.LogDebug("Attempting to update event with ID {DinnerId} for user ID {UserId}.", dinnerId, userIdFromToken);

        var existingDinner = await _dinnerRepository.GetByIdAsync(dinnerId);
        if (existingDinner == null)
        {
            _logger.LogWarning("Event with ID {DinnerId} not found.", dinnerId);
            throw new KeyNotFoundException($"Event with ID {dinnerId} not found.");
        }

        if (existingDinner.UserId != userIdFromToken)
        {
            _logger.LogWarning("Unauthorized update attempt by User ID {UserId} on Event ID {Dinner}", userIdFromToken, dinnerId);
            throw new UnauthorizedAccessException($"User ID {userIdFromToken} is not authorized to update event ID {dinnerId}.");
        }

        var dinnerToUpdate = _dinnerMapper.MapToModel(dinnerDTO);
        dinnerToUpdate.Id = dinnerId;

        var updatedDinner = await _dinnerRepository.UpdateAsync(dinnerId, dinnerToUpdate);
        if (updatedDinner == null)
        {
            _logger.LogError("Failed to update Dinner with ID {DinnerId}", dinnerId);
            throw new InvalidOperationException($"Failed to update Dinner with ID {dinnerId}.");
        }

        _logger.LogInformation("Dinner with ID {DinnerId} updated successfully by User ID {UserId}.", dinnerId, userIdFromToken);
        return _dinnerMapper.MapToDTO(updatedDinner);

    }

    // Deletes a dinner asynchronously.
    public async Task<DinnerDTO?> DeleteAsync(int userIdFromToken, int dinnerId)
    {
        _logger.LogDebug("Attempting to Dinner dinner with ID {DinnerId} by user ID {UserId}.", dinnerId, userIdFromToken);

        var dinnerToDelete = await _dinnerRepository.GetByIdAsync(dinnerId);

        if (dinnerToDelete == null)
        {
            _logger.LogWarning("Attempt to delete a non-existent Dinner with ID {DinnerId}.", dinnerId);
            throw new KeyNotFoundException($"Dinner with ID {dinnerId} not found.");
        }

        if (dinnerToDelete.Id != userIdFromToken)
        {
            _logger.LogWarning("Unauthorized delete attempt by User ID {UserId} on Event ID {DinnerId}.", userIdFromToken, dinnerId);
            throw new UnauthorizedAccessException($"User ID {userIdFromToken} is not authorized to delete Dinner ID {dinnerId}.");
        }

        var deletedDinner = await _dinnerRepository.DeleteAsync(dinnerId);

        if (deletedDinner == null)
        {
            _logger.LogError("Failed to delete Dinner with ID {DinnerId}.", dinnerId);
            throw new InvalidOperationException("Deletion failed, could not complete operation on the database.");
        }

        _logger.LogInformation("Dinner with ID {DinnerId} deleted successfully by User ID {UserId}.", dinnerId, userIdFromToken);
        return _dinnerMapper.MapToDTO(dinnerToDelete);


    }
}
