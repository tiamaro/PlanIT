namespace PlanIT.API.Repositories.Interfaces;

public interface IRepository<TEntity>
{
    // CREATE
    Task<TEntity?> AddAsync(TEntity entity);

    // READ
    Task<TEntity?> GetByIdAsync(int id);
    Task<ICollection<TEntity>> GetAllAsync(int pageNr, int pageSize);


    // UPDATE
    Task<TEntity?> UpdateAsync(int id, TEntity entity);

    // DELETE
    Task<TEntity?> DeleteAsync(int id);
}