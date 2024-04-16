using Microsoft.EntityFrameworkCore;
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

        return addedTodo?.Entity;
    }

    // Retrieves all todo items with pagination
    public async Task<ICollection<ToDo>> GetAllAsync(int pageNr, int pageSize)
    {
        IQueryable<ToDo> todosQuery = _dbContext.Todos.OrderBy(x => x.Id);
        return await _pagination.GetPageAsync(todosQuery, pageNr, pageSize);
    }

    // Retrieves a todo item by its ID
    public async Task<ToDo?> GetByIdAsync(int toDoId)
    {
        var exsistingToDo = await _dbContext.Todos.FirstOrDefaultAsync(x => x.Id == toDoId);
        return exsistingToDo is null ? null : exsistingToDo;
    }

    public async Task<ToDo?> UpdateAsync(int toDoId, ToDo updatedTodo)
    {
        var exsistingToDo = await _dbContext.Todos.FirstOrDefaultAsync(x => x.Id == toDoId);
        if (exsistingToDo == null) return null;

        exsistingToDo.Name = string.IsNullOrEmpty(updatedTodo.Name) ? exsistingToDo.Name : updatedTodo.Name;

        await _dbContext.SaveChangesAsync();
        return exsistingToDo;

    }

    // Deletes a todo item by its ID
    public async Task<ToDo?> DeleteAsync(int toDoId)
    {

        var exsistingToDO = await _dbContext.Todos.FindAsync(toDoId);
        if (exsistingToDO == null) return null;

        var deletedToDo = _dbContext.Todos.Remove(exsistingToDO);
        await _dbContext.SaveChangesAsync();


        return deletedToDo?.Entity;
    }
}
