using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Services.Interfaces;

namespace PlanIT.API.Services;

public class TodoService : IService<ToDoDTO>
{
    private readonly IRepository<ToDo> _todoRepository;
    private readonly IMapper<ToDo, ToDoDTO> _todoMapper;
    private readonly ILogger<TodoService> _logger;

    // Initializes a new instance of the TodoService class.
    public TodoService(
        IRepository<ToDo> todoRepository,
        IMapper<ToDo, ToDoDTO> todoMapper,
        ILogger<TodoService> logger)
    {
        _todoRepository = todoRepository;
        _todoMapper = todoMapper;
        _logger = logger;
    }

    // Creates a new todo asynchronously.
    public async Task<ToDoDTO?> CreateAsync(ToDoDTO todoDTO)
    {
        var newTodo = _todoMapper.MapToModel(todoDTO);
        var addedTodo = await _todoRepository.AddAsync(newTodo);
        return addedTodo != null ? _todoMapper.MapToDTO(addedTodo) : null;  
    }


    // Retrieves all todo items with pagination asynchronously.
    public async Task<ICollection<ToDoDTO>> GetAllAsync(int pageNr, int pageSize)
    {
        var toDosFromRepository = await _todoRepository.GetAllAsync(pageNr, pageSize);
        var todDoDTOs = toDosFromRepository.Select(todoEntity => _todoMapper.MapToDTO(todoEntity)).ToList();
        return todDoDTOs;
    }

    // Retrieves a single todo item by its unique identifier asynchronously.
    public async Task<ToDoDTO?> GetByIdAsync(int toDoId)
    {
        var toDoFromRepository = await _todoRepository.GetByIdAsync(toDoId);
        return toDoFromRepository != null ? _todoMapper.MapToDTO(toDoFromRepository) : null;
    }

    // Updates an existing todo item asynchronously.
    public async Task<ToDoDTO?> UpdateAsync(int id, ToDoDTO todoDto)
    {
        var existingTodo = await _todoRepository.GetByIdAsync(id);
        if (existingTodo == null) return null;

        var todoToUpdate = _todoMapper.MapToModel(todoDto);
        todoToUpdate.Id = id;

        var updatedTodo = await _todoRepository.UpdateAsync(id, todoToUpdate);
        return updatedTodo != null ? _todoMapper.MapToDTO(updatedTodo) : null;
    }

    // Deletes a todo item by its unique identifier asynchronously.
    public async Task<ToDoDTO?> DeleteAsync(int toDoId)
    {
        var toDoToDelete = await _todoRepository.GetByIdAsync(toDoId);
        if (toDoToDelete == null) return null;

        var deltedToDo = await _todoRepository.DeleteAsync(toDoId);
        return deltedToDo != null ? _todoMapper.MapToDTO(toDoToDelete) : null; 
    }
}
