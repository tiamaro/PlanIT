namespace PlanIT.API.Services.Interfaces;

public interface IService<TDto>
{
    // CREATE
    Task<TDto?> CreateAsync(int userIdFromToken, TDto dto);

    // READ
    Task<TDto?> GetByIdAsync(int userIdFromToken, int id);
    Task<ICollection<TDto>> GetAllAsync(int userIdFromToken, int pageNr, int pageSize);

    // UPDATE
    Task<TDto?> UpdateAsync(int userIdFromToken, int id, TDto dto);

    // DELETE
    Task<TDto?> DeleteAsync(int userIdFromToken, int id);
}