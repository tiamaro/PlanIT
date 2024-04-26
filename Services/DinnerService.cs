using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Services.Interfaces;
using PlanIT.API.Utilities;

namespace PlanIT.API.Services;

// Service class for handling dinner information.
// Exceptions are caught by a middleware: HandleExceptionFilter
public class DinnerService : IService<DinnerDTO>
{
    private readonly IMapper<Dinner, DinnerDTO> _dinnerMapper;
    private readonly IRepository<Dinner> _dinnerRepository;
    private readonly LoggerService _logger;

    public DinnerService(IMapper<Dinner, DinnerDTO> dinnerMapper,
        IRepository<Dinner> dinnerRepository,
        LoggerService logger)
    {
        _dinnerMapper = dinnerMapper;
        _dinnerRepository = dinnerRepository;
        _logger = logger;
    }


    
    public async Task<DinnerDTO?> CreateAsync(int userIdFromToken, DinnerDTO dinnerDTO)
    {
        _logger.LogCreationStart("dinner");

        var newDinner = _dinnerMapper.MapToModel(dinnerDTO);
        newDinner.UserId = userIdFromToken;

        
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
        
        var dinnersFromRepository = await _dinnerRepository.GetAllAsync(1, 10);
        
       
        var filteredDinners = dinnersFromRepository.Where(dinner => dinner.UserId == userIdFromToken);

       
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

       
        if (dinnerFromRepository.UserId != userIdFromToken)
        {
            _logger.LogUnauthorizedAccess("dinner", dinnerId, userIdFromToken);
            throw ExceptionHelper.CreateUnauthorizedException("dinner", dinnerId);
        }

        
        _logger.LogOperationSuccess("retrieved", "dinner", dinnerId);
        return _dinnerMapper.MapToDTO(dinnerFromRepository);
    }


    
    public async Task<DinnerDTO?> UpdateAsync(int userIdFromToken, int dinnerId, DinnerDTO dinnerDTO)
    {
        _logger.LogDebug($"Oppdaterer middag med ID {dinnerId} for bruker {userIdFromToken}.");

        
        var existingDinner = await _dinnerRepository.GetByIdAsync(dinnerId);
        if (existingDinner == null)
        {
            _logger.LogNotFound("dinner", dinnerId);
            throw ExceptionHelper.CreateNotFoundException("dinner", dinnerId);
        }

       
        if (existingDinner.UserId != userIdFromToken)
        {
            _logger.LogUnauthorizedAccess("dinner", dinnerId, userIdFromToken);
            throw ExceptionHelper.CreateUnauthorizedException("dinner", dinnerId);
        }

        var dinnerToUpdate = _dinnerMapper.MapToModel(dinnerDTO);
        dinnerToUpdate.Id = dinnerId;  

        
        var updatedDinner = await _dinnerRepository.UpdateAsync(dinnerId, dinnerToUpdate);
        if (updatedDinner == null)
        {
            _logger.LogOperationFailure("update", "dinner", dinnerId);
            throw ExceptionHelper.CreateOperationException("dinner", dinnerId, "update");
        }

        _logger.LogOperationSuccess("updated", "dinner", dinnerId);
        return _dinnerMapper.MapToDTO(updatedDinner);
    }


    
    public async Task<DinnerDTO?> DeleteAsync(int userIdFromToken, int dinnerId)
    {
        _logger.LogDebug($"Forsøker å slette middag med ID {dinnerId} av bruker {userIdFromToken}.");

        
        var dinnerToDelete = await _dinnerRepository.GetByIdAsync(dinnerId);
        if (dinnerToDelete == null)
        {
            _logger.LogNotFound("dinner", dinnerId);
            throw ExceptionHelper.CreateNotFoundException("dinner", dinnerId);
        }

        
        if (dinnerToDelete.UserId != userIdFromToken)
        {
            _logger.LogUnauthorizedAccess("dinner", dinnerId, userIdFromToken);
            throw ExceptionHelper.CreateUnauthorizedException("dinner", dinnerId);
        }

        
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