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
        return addedDinner != null ? _dinnerMapper.MapToDTO(addedDinner) : null;
    }

    // Retrieves all dinners with pagination asynchronously.
    public async Task<ICollection<DinnerDTO>> GetAllAsync(int pageNr, int pageSize)
    {
        var dinnersFromRepository = await _dinnerRepository.GetAllAsync(pageNr, pageSize);
        var dinnerDTOs = dinnersFromRepository.Select(dinnerEntity => _dinnerMapper.MapToDTO(dinnerEntity)).ToList();
        return dinnerDTOs;
    }

    // Retrieves a dinner by ID asynchronously.
    public async Task<DinnerDTO?> GetByIdAsync(int dinnerId)
    {
        var dinnerFromRepository = await _dinnerRepository.GetByIdAsync(dinnerId);
        return dinnerFromRepository != null ? _dinnerMapper.MapToDTO(dinnerFromRepository) : null;
    }
        
    // Updates a dinner asynchronously.
    public async Task<DinnerDTO?> UpdateAsync(int dinnerId, DinnerDTO dinnerDTO)
    {
        var existingDinner = await _dinnerRepository.GetByIdAsync(dinnerId);
        if (existingDinner == null) return null;

        var dinnerToUpdate = _dinnerMapper.MapToModel(dinnerDTO);
        dinnerToUpdate.Id = dinnerId;

        var updatedDinner = await _dinnerRepository.UpdateAsync(dinnerId, dinnerToUpdate);
        return updatedDinner != null ? _dinnerMapper.MapToDTO(updatedDinner) : null;
    }

    // Deletes a dinner asynchronously.
    public async Task<DinnerDTO?> DeleteAsync(int dinnerId)
    {
        var dinnerToDelete = await _dinnerRepository.GetByIdAsync(dinnerId);
        if (dinnerToDelete == null) return null;

        var deletedDinner = await _dinnerRepository.DeleteAsync(dinnerId);
        return deletedDinner != null ? _dinnerMapper.MapToDTO(dinnerToDelete) : null;
    }
}
