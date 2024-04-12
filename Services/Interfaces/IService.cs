namespace PlanIT.API.Services.Interfaces;

public interface IService<TDto>
{
    // CREATE

    Task<TDto?> CreateAsync(TDto dto);

    // READ
    Task<TDto?> GetByIdAndUserIdAsync(int id, int userId);
    Task<ICollection<TDto>> GetAllAsync(int pageNr, int pageSize);

    // UPDATE
    Task<TDto?> UpdateAsync(int id, TDto dto);

    // DELETE
    Task<TDto?> DeleteAsync(int id);
}