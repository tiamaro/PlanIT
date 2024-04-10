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
    public async Task<ImportantDateDTO?> CreateAsync(ImportantDateDTO importantDateDTO)
    {

        var newImportantDate = _dateMapper.MapToModel(importantDateDTO);
        var addedImportantDate = await _dateRepository.AddAsync(newImportantDate);
        return addedImportantDate != null ? _dateMapper.MapToDTO(addedImportantDate) : null;
    }


    // Henter alle ImportantDates med paginering
    public async Task<ICollection<ImportantDateDTO>> GetAllAsync(int pageNr, int pageSize)
    {
        var importantDatesFromRepository = await _dateRepository.GetAllAsync(1, 10);
        var importantDateDTOs = importantDatesFromRepository.Select(dateEntity => _dateMapper.MapToDTO(dateEntity)).ToList();
        return importantDateDTOs;
    }


    // Henter ImportantDate basert på ID
    public async Task<ImportantDateDTO?> GetByIdAsync(int importantDateId)
    {
        var importantDatesFromRepository = await _dateRepository.GetByIdAsync(importantDateId);
        return importantDatesFromRepository != null ? _dateMapper.MapToDTO(importantDatesFromRepository) : null;
    }

    // Oppdaterer ImportantDate
    public async Task<ImportantDateDTO?> UpdateAsync(int importantDateId, ImportantDateDTO dateDTO)
    {
        var exsistingImportantDate = await _dateRepository.GetByIdAsync(importantDateId);

        if (exsistingImportantDate == null) return null;
        
        var importantDateToUpdate = _dateMapper.MapToModel(dateDTO);
        importantDateToUpdate.Id = importantDateId;

        var updatedImportantDate = await _dateRepository.UpdateAsync(importantDateId, importantDateToUpdate);
        return updatedImportantDate != null ? _dateMapper.MapToDTO(updatedImportantDate) : null;
    }


    // Sletter ImportantDate
    public async Task<ImportantDateDTO?> DeleteAsync(int importantDateId)
    {
        var importantDateToDelete = await _dateRepository.GetByIdAsync(importantDateId);
        if (importantDateToDelete == null) return null;

        var deltedImportantDate = await _dateRepository.DeleteAsync(importantDateId);
        return deltedImportantDate != null ? _dateMapper.MapToDTO(importantDateToDelete) : null;
    }
}
