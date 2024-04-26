using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Services.Interfaces;
using PlanIT.API.Utilities;

namespace PlanIT.API.Services;

// Service class for handling important dates information.
// Exceptions are caught by a middleware: HandleExceptionFilter
public class ImportantDateService : IService<ImportantDateDTO>
{
    private readonly IMapper<ImportantDate, ImportantDateDTO> _dateMapper;
    private readonly IRepository<ImportantDate> _dateRepository;
    private readonly LoggerService _logger;

    public ImportantDateService(IMapper<ImportantDate, ImportantDateDTO> dateMapper,
        IRepository<ImportantDate> dateRepository,
        LoggerService logger)
    {
        _dateMapper = dateMapper;
        _dateRepository = dateRepository;
        _logger = logger;
    }


    
    public async Task<ImportantDateDTO?> CreateAsync(int userIdFromToken, ImportantDateDTO newImportantDateDTO)
    {
        _logger.LogCreationStart("important date");

        
        var newImportantDate = _dateMapper.MapToModel(newImportantDateDTO);
        newImportantDate.UserId = userIdFromToken;

        
        var addedImportantDate = await _dateRepository.AddAsync(newImportantDate);
        if (addedImportantDate == null)
        {
            _logger.LogCreationFailure("important date");
            throw ExceptionHelper.CreateOperationException("important date", 0, "create");
        }

        _logger.LogOperationSuccess("created", "important date", addedImportantDate.Id);
        return _dateMapper.MapToDTO(addedImportantDate);
    }


   
    public async Task<ICollection<ImportantDateDTO>> GetAllAsync(int userIdFromToken,int pageNr, int pageSize)
    {

        
        var importantDatesFromRepository = await _dateRepository.GetAllAsync(1, 10);
      
        
        var filteredImportantDates = importantDatesFromRepository.Where(importantDate => importantDate.UserId == userIdFromToken);

        
        return filteredImportantDates.Select(importantDateEntity => _dateMapper.MapToDTO(importantDateEntity)).ToList();


    }


    
    public async Task<ImportantDateDTO?> GetByIdAsync(int userIdFromToken, int importantDateId)
    {
        _logger.LogDebug("Henter viktig dato med ID {ImportantDateId} for bruker ID {UserId}.", importantDateId, userIdFromToken);

        var importantDateFromRepository = await _dateRepository.GetByIdAsync(importantDateId);
        if (importantDateFromRepository == null)
        {
            _logger.LogNotFound("important date", importantDateId);
            throw ExceptionHelper.CreateNotFoundException("important date", importantDateId);
        }

        
        if (importantDateFromRepository.UserId != userIdFromToken)
        {
            _logger.LogUnauthorizedAccess("important date", importantDateId, userIdFromToken);
            throw ExceptionHelper.CreateUnauthorizedException("important date", importantDateId);
        }

        
        _logger.LogOperationSuccess("retrieved", "important date", importantDateId);
        return _dateMapper.MapToDTO(importantDateFromRepository);
    }


    
    public async Task<ImportantDateDTO?> UpdateAsync(int userIdFromToken, int importantDateId, ImportantDateDTO dateDTO)
    {
        _logger.LogDebug("Oppdaterer viktig dato med ID {ImportantDateId} for bruker ID {UserId}.", importantDateId, userIdFromToken);


        
        var existingImportantDate = await _dateRepository.GetByIdAsync(importantDateId);
        if (existingImportantDate == null)
        {
            _logger.LogNotFound("important date", importantDateId);
            throw ExceptionHelper.CreateNotFoundException("important date", importantDateId);
        }

        
        if (existingImportantDate.UserId != userIdFromToken)
        {
            _logger.LogUnauthorizedAccess("important date", importantDateId, userIdFromToken);
            throw ExceptionHelper.CreateUnauthorizedException("important date", importantDateId);
        }

        var importantDateToUpdate = _dateMapper.MapToModel(dateDTO);
        importantDateToUpdate.Id = importantDateId;  

        
        var updatedImportantDate = await _dateRepository.UpdateAsync(importantDateId, importantDateToUpdate);
        if (updatedImportantDate == null)
        {
            _logger.LogOperationFailure("update", "important date", importantDateId);
            throw ExceptionHelper.CreateOperationException("important date", importantDateId, "update");
        }

        _logger.LogOperationSuccess("updated", "important date", importantDateId);
        return _dateMapper.MapToDTO(updatedImportantDate);
    }


    
    public async Task<ImportantDateDTO?> DeleteAsync(int userIdFromToken, int importantDateId)
    {
        _logger.LogDebug("Forsøker å slette viktig dato med ID {ImportantDateId} av bruker {UserId}.", importantDateId, userIdFromToken);

        
        var importantDateToDelete = await _dateRepository.GetByIdAsync(importantDateId);
        if (importantDateToDelete == null)
        {
            _logger.LogNotFound("important date", importantDateId);
            throw ExceptionHelper.CreateNotFoundException("important date", importantDateId);
        }

        
        if (importantDateToDelete.UserId != userIdFromToken)
        {
            _logger.LogUnauthorizedAccess("important date", importantDateId, userIdFromToken);
            throw ExceptionHelper.CreateUnauthorizedException("important date", importantDateId);
        }

        
        var deletedImportantDate = await _dateRepository.DeleteAsync(importantDateId);
        if (deletedImportantDate == null)
        {
            _logger.LogOperationFailure("delete", "important date", importantDateId);
            throw ExceptionHelper.CreateOperationException("important date", importantDateId, "delete");
        }

        _logger.LogOperationSuccess("deleted", "important date", importantDateId);
        return _dateMapper.MapToDTO(deletedImportantDate);
    }
}
