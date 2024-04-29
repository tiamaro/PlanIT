using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Services.Interfaces;
using PlanIT.API.Utilities;

namespace PlanIT.API.Services;


// Serviceklasse for håndtering av middagsinformasjon.
// Exceptions blir fanget av en middleware: HandleExceptionFilter
public class DinnerService : IDinnerService
{
    private readonly IMapper<Dinner, DinnerDTO> _dinnerMapper;
    private readonly IWeeklyDinnerPlanMapper _weeklyDinnerPlanMapper;
    private readonly IDinnerRepository _dinnerRepository;
    private readonly LoggerService _logger;

    public DinnerService(IMapper<Dinner, DinnerDTO> dinnerMapper,
        IWeeklyDinnerPlanMapper weeklyDinnerPlanMapper,
        IDinnerRepository dinnerRepository,
        LoggerService logger)
    {
        _dinnerMapper = dinnerMapper;
        _weeklyDinnerPlanMapper = weeklyDinnerPlanMapper;
        _dinnerRepository = dinnerRepository;
        _logger = logger;
    }

    public async Task<DinnerDTO?> CreateAsync(int userIdFromToken, DinnerDTO dinnerDTO)
    {
        _logger.LogCreationStart("dinner");

        var dinnerDate = dinnerDTO.Date;

        // Ensure the dinner date is not in the past
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (dinnerDate < today)
        {
            _logger.LogWarning("Attempt to register dinner with a past date.");
            return null;
        }

        // Check for existing dinner on the same date
        var existingDinner = await _dinnerRepository.GetByDayAndUserAsync(userIdFromToken, dinnerDate);
        if (existingDinner != null)
        {
            _logger.LogWarning("Attempt to register a duplicate dinner on the same date.");
            return null; 
        }

        var newDinner = _dinnerMapper.MapToModel(dinnerDTO);
        newDinner.UserId = userIdFromToken;
        newDinner.Date = dinnerDate; // Ensure date is stored correctly

        var addedDinner = await _dinnerRepository.AddAsync(newDinner);
        if (addedDinner == null)
        {
            _logger.LogCreationFailure("dinner");
            throw ExceptionHelper.CreateOperationException("dinner", 0, "create");
        }

        _logger.LogOperationSuccess("created", "dinner", addedDinner.Id);
        return _dinnerMapper.MapToDTO(addedDinner);
    }


    public async Task<ICollection<DinnerDTO>> GetAllAsync(int userIdFromToken,int pageNr, int pageSize)
    {
        // Henter Contacts fra repository med paginering
        var dinnersFromRepository = await _dinnerRepository.GetAllAsync(pageNr, pageSize);
        
        // filter
        var filteredDinners = dinnersFromRepository.Where(dinner => dinner.UserId == userIdFromToken);

        // Mapper Contactsinformasjon til contactsDTO-format
        return filteredDinners.Select(dinnerEntity => _dinnerMapper.MapToDTO(dinnerEntity)).ToList();
    }


    public async Task<DinnerDTO?> GetByIdAsync(int userIdFromToken, int dinnerId)
    {
        _logger.LogDebug($"Henter middag med ID {dinnerId} for bruker {userIdFromToken}.");

        var dinnerFromRepository = await _dinnerRepository.GetByIdAsync(dinnerId);
        if (dinnerFromRepository == null)
        {
            _logger.LogNotFound("dinner", dinnerId);
            throw ExceptionHelper.CreateNotFoundException("dinner", dinnerId);
        }

        // Sjekker om brukerens ID stemmer overens med brukerID tilknyttet middagen
        if (dinnerFromRepository.UserId != userIdFromToken)
        {
            _logger.LogUnauthorizedAccess("dinner", dinnerId, userIdFromToken);
            throw ExceptionHelper.CreateUnauthorizedException("dinner", dinnerId);
        }

        // Logger vellykket henting av middagen
        _logger.LogOperationSuccess("retrieved", "dinner", dinnerId);
        return _dinnerMapper.MapToDTO(dinnerFromRepository);
    }



    public async Task<WeeklyDinnerPlanDTO> GetWeeklyDinnerPlanAsync(int userIdFromToken, DateOnly startDate, DateOnly endDate)
    {
        _logger.LogDebug("Fetching weekly dinner plan for user {UserId} from {StartDate} to {EndDate}.", userIdFromToken, startDate, endDate);

        // Fetching dinners within the specified date range for the given user
        var dinnersFromRepository = await _dinnerRepository.GetByDateRangeAndUserAsync(userIdFromToken, startDate, endDate);

        if (dinnersFromRepository == null)
        {
            _logger.LogNotFound("dinners", userIdFromToken);
            throw ExceptionHelper.CreateNotFoundException("dinners for user", userIdFromToken);
        }

        _logger.LogOperationSuccess("retrieved", "weekly dinner plan", userIdFromToken);
        return _weeklyDinnerPlanMapper.MapToDTO(dinnersFromRepository);

    }


    public async Task<DinnerDTO?> UpdateAsync(int userIdFromToken, int dinnerId, DinnerDTO dinnerDTO)
    {
        _logger.LogDebug($"Updating dinner with ID {dinnerId} for user {userIdFromToken}.");

        var existingDinner = await _dinnerRepository.GetByIdAsync(dinnerId);
        if (existingDinner == null || existingDinner.UserId != userIdFromToken)
        {
            _logger.LogNotFound("dinner", dinnerId);
            throw ExceptionHelper.CreateNotFoundException("dinner", dinnerId);
        }

        // Ensure that updates do not change the date
        if (dinnerDTO.Date != existingDinner.Date)
        {
            _logger.LogWarning("Attempt to change the date of an existing dinner.");
            throw ExceptionHelper.CreateOperationException("dinner", dinnerId, "attempt to change date");
        }

        existingDinner.Name = !string.IsNullOrEmpty(dinnerDTO.Name) ? dinnerDTO.Name : existingDinner.Name;

        var updatedDinner = await _dinnerRepository.UpdateAsync(dinnerId, existingDinner);
        if (updatedDinner == null)
        {
            _logger.LogOperationFailure("update", "dinner", dinnerId);
            throw ExceptionHelper.CreateOperationException("dinner", dinnerId, "update");
        }

        _logger.LogOperationSuccess("updated", "dinner", dinnerId);
        return _dinnerMapper.MapToDTO(updatedDinner);
    }



    // Sletter en middag
    public async Task<DinnerDTO?> DeleteAsync(int userIdFromToken, int dinnerId)
    {
        _logger.LogDebug($"Forsøker å slette middag med ID {dinnerId} av bruker {userIdFromToken}.");

        // Henter middagen fra databasen for å sikre at den eksisterer før sletting
        var dinnerToDelete = await _dinnerRepository.GetByIdAsync(dinnerId);
        if (dinnerToDelete == null)
        {
            _logger.LogNotFound("dinner", dinnerId);
            throw ExceptionHelper.CreateNotFoundException("dinner", dinnerId);
        }

        // Sjekker om brukeren har riktig autorisasjon til å slette middagen
        if (dinnerToDelete.UserId != userIdFromToken)
        {
            _logger.LogUnauthorizedAccess("dinner", dinnerId, userIdFromToken);
            throw ExceptionHelper.CreateUnauthorizedException("dinner", dinnerId);
        }

        // Utfører slettingen av middagen fra databasen
        var deletedDinner = await _dinnerRepository.DeleteAsync(dinnerId);
        if (deletedDinner == null)
        {
            _logger.LogOperationFailure("delete", "dinner", dinnerId);
            throw ExceptionHelper.CreateOperationException("dinner", dinnerId, "delete");
        }

        _logger.LogOperationSuccess("deleted", "dinner", dinnerId);
        return _dinnerMapper.MapToDTO(deletedDinner);
    }

}