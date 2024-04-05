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
        _todoRepository = todoRepository ?? throw new ArgumentNullException(nameof(todoRepository));
        _todoMapper = todoMapper ?? throw new ArgumentNullException(nameof(todoMapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // Creates a new todo asynchronously.
    public async Task<ToDoDTO?> CreateAsync(ToDoDTO newDto)
    {
        var newTodo = _todoMapper.MapToModel(newDto);
        var addedTodo = await _todoRepository.AddAsync(newTodo);
        return _todoMapper.MapToDTO(addedTodo!);
    }

    // Retrieves a single todo item by its unique identifier asynchronously.
    public async Task<ToDoDTO?> GetByIdAsync(int id)
    {
        var todo = await _todoRepository.GetByIdAsync(id);
        return todo != null ? _todoMapper.MapToDTO(todo) : null;
    }

    // Retrieves all todo items with pagination asynchronously.
    public async Task<ICollection<ToDoDTO>> GetAllAsync(int pageNr, int pageSize)
    {
        var todos = await _todoRepository.GetAllAsync(pageNr, pageSize);
        return todos.Select(_todoMapper.MapToDTO).ToList();
    }

    // Updates an existing todo item asynchronously.
    public async Task<ToDoDTO?> UpdateAsync(int id, ToDoDTO todoDto)
    {
        var existingTodo = await _todoRepository.GetByIdAsync(id);
        if (existingTodo == null)
        {
            _logger.LogWarning("Todo item with ID {TodoId} was not found.", id);
            return null;
        }

        var todoToUpdate = _todoMapper.MapToModel(todoDto);
        todoToUpdate.Id = id;

        var updatedTodo = await _todoRepository.UpdateAsync(id, todoToUpdate);
        return updatedTodo != null ? _todoMapper.MapToDTO(updatedTodo) : null;
    }

    // Deletes a todo item by its unique identifier asynchronously.
    public async Task<ToDoDTO?> DeleteAsync(int id)
    {
        var deletedTodo = await _todoRepository.DeleteAsync(id);
        if (deletedTodo == null)
        {
            _logger.LogWarning("Todo with ID {TodoId} was not found.", id);
            return null;
        }

        return _todoMapper.MapToDTO(deletedTodo);
    }
}
