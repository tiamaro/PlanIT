namespace PlanIT.API.Services.Interfaces;

public interface IService<TDto>
{
    // CREATE

    Task<TDto?> CreateAsync(TDto dto);

    // READ
    Task<TDto?> GetByIdAsync(int userId, int id);
    Task<ICollection<TDto>> GetAllAsync(int pageNr, int pageSize);

    // UPDATE
    Task<TDto?> UpdateAsync(int userId, int id, TDto dto);

    // DELETE
    Task<TDto?> DeleteAsync(int userid, int id);
}