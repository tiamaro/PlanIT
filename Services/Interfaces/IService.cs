namespace PlanIT.API.Services.Interfaces;

public interface IService<TDto>
{
    // CREATE
    Task<TDto?> CreateAsync(TDto newDto);

    // READ
    Task<TDto?> GetByIdAsync(int id);
    Task<ICollection<TDto>> GetAllAsync(int pageNr, int pageSize);

    // UPDATE
    Task<TDto?> UpdateAsync(int id, TDto dto);

    // DELETE
    Task<TDto?> DeleteAsync(int id);
}