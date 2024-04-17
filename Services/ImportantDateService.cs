using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Services.Interfaces;
using PlanIT.API.Utilities; // Inkluderer tilgang til LoggerService og ExceptionHelper

namespace PlanIT.API.Services;

// Serviceklasse for håndtering av viktige datoer.
// Exceptions blir fanget av en middleware: HandleExceptionFilter
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


    // Oppretter en ny viktig dato
    public async Task<ImportantDateDTO?> CreateAsync(ImportantDateDTO newImportantDateDTO)
    {
        _logger.LogCreationStart("important date");

        // Mapper DTO til domenemodell
        var newImportantDate = _dateMapper.MapToModel(newImportantDateDTO);

        // Legger til den nye viktige datoen i databasen
        var addedImportantDate = await _dateRepository.AddAsync(newImportantDate);
        if (addedImportantDate == null)
        {
            _logger.LogCreationFailure("important date");
            throw ExceptionHelper.CreateOperationException("important date", 0, "create");
        }

        _logger.LogOperationSuccess("created", "important date", addedImportantDate.Id);
        return _dateMapper.MapToDTO(addedImportantDate);
    }


    // Henter alle viktige datoer med paginering
    public async Task<ICollection<ImportantDateDTO>> GetAllAsync(int pageNr, int pageSize)
    {
        var importantDatesFromRepository = await _dateRepository.GetAllAsync(pageNr, pageSize);
        return importantDatesFromRepository.Select(_dateMapper.MapToDTO).ToList();
    }


    // Henter en spesifikk viktig dato basert på ID
    public async Task<ImportantDateDTO?> GetByIdAsync(int userIdFromToken, int importantDateId)
    {
        _logger.LogDebug("Henter viktig dato med ID {ImportantDateId} for bruker ID {UserId}.", importantDateId, userIdFromToken);

        var importantDateFromRepository = await _dateRepository.GetByIdAsync(importantDateId);
        if (importantDateFromRepository == null)
        {
            _logger.LogNotFound("important date", importantDateId);
            throw ExceptionHelper.CreateNotFoundException("important date", importantDateId);
        }

        // Sjekker om brukerens ID stemmer overens med brukerID tilknyttet den vikitge datoen
        if (importantDateFromRepository.UserId != userIdFromToken)
        {
            _logger.LogUnauthorizedAccess("important date", importantDateId, userIdFromToken);
            throw ExceptionHelper.CreateUnauthorizedException("important date", importantDateId);
        }

        // Logger vellykket henting av datoen
        _logger.LogOperationSuccess("retrieved", "important date", importantDateId);
        return _dateMapper.MapToDTO(importantDateFromRepository);
    }


    // Oppdaterer en viktig dato
    public async Task<ImportantDateDTO?> UpdateAsync(int userIdFromToken, int importantDateId, ImportantDateDTO dateDTO)
    {
        _logger.LogDebug("Oppdaterer viktig dato med ID {ImportantDateId} for bruker ID {UserId}.", importantDateId, userIdFromToken);


        // Forsøker å hente den eksisterende datoen fra databasen
        var existingImportantDate = await _dateRepository.GetByIdAsync(importantDateId);
        if (existingImportantDate == null)
        {
            _logger.LogNotFound("important date", importantDateId);
            throw ExceptionHelper.CreateNotFoundException("important date", importantDateId);
        }

        // Sjekker om brukeren har autorisasjon til å oppdatere datoen
        if (existingImportantDate.UserId != userIdFromToken)
        {
            _logger.LogUnauthorizedAccess("important date", importantDateId, userIdFromToken);
            throw ExceptionHelper.CreateUnauthorizedException("important date", importantDateId);
        }

        var importantDateToUpdate = _dateMapper.MapToModel(dateDTO);
        importantDateToUpdate.Id = importantDateId;  // Sikrer at ID ikke endres under oppdateringen

        // Utfører oppdateringen i databasen 
        var updatedImportantDate = await _dateRepository.UpdateAsync(importantDateId, importantDateToUpdate);
        if (updatedImportantDate == null)
        {
            _logger.LogOperationFailure("update", "important date", importantDateId);
            throw ExceptionHelper.CreateOperationException("important date", importantDateId, "update");
        }

        _logger.LogOperationSuccess("updated", "important date", importantDateId);
        return _dateMapper.MapToDTO(updatedImportantDate);
    }


    // Sletter en viktig dato
    public async Task<ImportantDateDTO?> DeleteAsync(int userIdFromToken, int importantDateId)
    {
        _logger.LogDebug("Forsøker å slette viktig dato med ID {ImportantDateId} av bruker {UserId}.", importantDateId, userIdFromToken);

        // Henter datoen fra databasen for å sikre at den eksisterer før sletting
        var importantDateToDelete = await _dateRepository.GetByIdAsync(importantDateId);
        if (importantDateToDelete == null)
        {
            _logger.LogNotFound("important date", importantDateId);
            throw ExceptionHelper.CreateNotFoundException("important date", importantDateId);
        }

        // Sjekker om brukeren har riktig autorisasjon til å slette datoen
        if (importantDateToDelete.UserId != userIdFromToken)
        {
            _logger.LogUnauthorizedAccess("important date", importantDateId, userIdFromToken);
            throw ExceptionHelper.CreateUnauthorizedException("important date", importantDateId);
        }

        // Utfører slettingen av datoen fra databasen
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
