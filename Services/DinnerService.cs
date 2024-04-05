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

    public DinnerService(IMapper<Dinner, DinnerDTO> dinnerMapper, IRepository<Dinner> dinnerRepository)
    {
        _dinnerMapper = dinnerMapper;
        _dinnerRepository = dinnerRepository;
    }

    // Creates a new dinner asynchronously.
    public async Task<DinnerDTO?> CreateAsync(DinnerDTO newDto)
    {
        var newDinner = _dinnerMapper.MapToModel(newDto);
        var addedDinner = await _dinnerRepository.AddAsync(newDinner);
        return addedDinner != null ? _dinnerMapper.MapToDTO(addedDinner) : null;
    }

    // Retrieves all dinners with pagination asynchronously.
    public async Task<ICollection<DinnerDTO>> GetAllAsync(int pageNr, int pageSize)
    {
        var dinners = await _dinnerRepository.GetAllAsync(pageNr, pageSize);
        return dinners.Select(_dinnerMapper.MapToDTO).ToList();
    }

    // Retrieves a dinner by ID asynchronously.
    public async Task<DinnerDTO?> GetByIdAsync(int id)
    {
        var dinner = await _dinnerRepository.GetByIdAsync(id);
        return dinner != null ? _dinnerMapper.MapToDTO(dinner) : null;
    }

    // Updates a dinner asynchronously.
    public async Task<DinnerDTO?> UpdateAsync(int id, DinnerDTO dto)
    {
        var updatedDinner = _dinnerMapper.MapToModel(dto);
        var result = await _dinnerRepository.UpdateAsync(id, updatedDinner);
        return result != null ? _dinnerMapper.MapToDTO(result) : null;
    }

    // Deletes a dinner asynchronously.
    public async Task<DinnerDTO?> DeleteAsync(int id)
    {
        var deletedDinner = await _dinnerRepository.DeleteAsync(id);
        return deletedDinner != null ? _dinnerMapper.MapToDTO(deletedDinner) : null;
    }
}
