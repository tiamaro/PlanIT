using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Services.Interfaces;

namespace PlanIT.API.Services;

public class TodoService : IService<TodoDTO>
{
	private readonly IRepository<Todo> _todoRepository;
	private readonly IMapper<Todo, TodoDTO> _todoMapper;
	private readonly ILogger<TodoService> _logger;

	// Initializes a new instance of the TodoService class.
	public TodoService(
		IRepository<Todo> todoRepository,
		IMapper<Todo, TodoDTO> todoMapper,
		ILogger<TodoService> logger)
	{
		_todoRepository = todoRepository ?? throw new ArgumentNullException(nameof(todoRepository));
		_todoMapper = todoMapper ?? throw new ArgumentNullException(nameof(todoMapper));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	// Creates a new todo asynchronously.
	public async Task<TodoDTO?> CreateAsync(TodoDTO newDto)
	{
		var newTodo = _todoMapper.MapToModel(newDto);
		var addedTodo = await _todoRepository.AddAsync(newTodo);
		return _todoMapper.MapToDTO(addedTodo!);
	}

	// Retrieves a single todo item by its unique identifier asynchronously.
	public async Task<TodoDTO?> GetByIdAsync(int id)
	{
		var todo = await _todoRepository.GetByIdAsync(id);
		return todo != null ? _todoMapper.MapToDTO(todo) : null;
	}

	// Retrieves all todo items with pagination asynchronously.
	public async Task<ICollection<TodoDTO>> GetAllAsync(int pageNr, int pageSize)
	{
		var todos = await _todoRepository.GetAllAsync(pageNr, pageSize);
		return todos.Select(_todoMapper.MapToDTO).ToList();
	}

	// Updates an existing todo item asynchronously.
	public async Task<TodoDTO?> UpdateAsync(int id, TodoDTO todoDto)
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
	public async Task<TodoDTO?> DeleteAsync(int id)
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
