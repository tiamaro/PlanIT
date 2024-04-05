using PlanIT.API.Data;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Utilities;

namespace PlanIT.API.Repositories;

public class TodoRepository : IRepository<ToDo>
{
    private readonly PlanITDbContext _dbContext;
    private readonly PaginationUtility _pagination;

    public TodoRepository(PlanITDbContext dbContext, PaginationUtility pagination)
    {
        _dbContext = dbContext;
        _pagination = pagination;
    }

    public async Task<ToDo?> AddAsync(ToDo newTodo)
    {
        var addedTodo = await _dbContext.Todos.AddAsync(newTodo);
        await _dbContext.SaveChangesAsync();
        return addedTodo.Entity;
    }

    // Retrieves all todo items with pagination
    public async Task<ICollection<ToDo>> GetAllAsync(int pageNr, int pageSize)
    {
        IQueryable<ToDo> todosQuery = _dbContext.Todos.OrderBy(x => x.Id);
        return await _pagination.GetPageAsync(todosQuery, pageNr, pageSize);
    }

    // Retrieves a todo item by its ID
    public async Task<ToDo?> GetByIdAsync(int id)
    {
        return await _dbContext.Todos.FindAsync(id);
    }

    public async Task<ToDo?> UpdateAsync(int id, ToDo updatedTodo)
    {
        // Find the existing todo item by its ID
        var existingTodo = await _dbContext.Todos.FindAsync(id);

        // If the existing todo item is not found, return null
        if (existingTodo == null)
        {
            return null;
        }

        // Update the properties of the existing todo item
        existingTodo.Name = updatedTodo.Name;

        // Save changes to the database
        await _dbContext.SaveChangesAsync();

        // Return the updated todo item
        return existingTodo;
    }

    // Deletes a todo item by its ID
    public async Task<ToDo?> DeleteAsync(int id)
    {
        // Find the todo item to delete by its ID
        var todoToDelete = await _dbContext.Todos.FindAsync(id);

        // If the todo item is not found, return null
        if (todoToDelete == null)
        {
            return null;
        }

        // Remove the todo item from the database
        _dbContext.Todos.Remove(todoToDelete);
        await _dbContext.SaveChangesAsync();

        // Return the deleted todo item
        return todoToDelete;
    }
}
