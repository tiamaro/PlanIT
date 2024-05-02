using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Services.Interfaces;
using PlanIT.API.Utilities; 

namespace PlanIT.API.Services;

// Service class for handling todo information.
// Exceptions are caught by a middleware: HandleExceptionFilter
public class TodoService : IService<ToDoDTO>
{
    private readonly IRepository<ToDo> _todoRepository;
    private readonly IMapper<ToDo, ToDoDTO> _todoMapper;
    private readonly LoggerService _logger;

    public TodoService(
        IRepository<ToDo> todoRepository,
        IMapper<ToDo, ToDoDTO> todoMapper,
        LoggerService logger)
    {
        _todoRepository = todoRepository;
        _todoMapper = todoMapper;
        _logger = logger;
    }

    
    public async Task<ToDoDTO?> CreateAsync(int userIdFromToken, ToDoDTO newToDoDTO)
    {
        _logger.LogCreationStart("todo");

        var newToDo = _todoMapper.MapToModel(newToDoDTO);

        newToDo.UserId = userIdFromToken;

        var addedToDo = await _todoRepository.AddAsync(newToDo);
        if (addedToDo == null)
        {
            _logger.LogCreationFailure("todo");
            throw ExceptionHelper.CreateOperationException("todo", 0, "create");
        }

        _logger.LogOperationSuccess("created", "todo", addedToDo.Id);
        return _todoMapper.MapToDTO(addedToDo);
    }



  
    public async Task<ICollection<ToDoDTO>> GetAllAsync(int userIdFromToken, int pageNr, int pageSize)
    {
        _logger.LogDebug($"Retrieving all todos for user {userIdFromToken}.");

        var ToDosFromRepository = await _todoRepository.GetAllAsync(pageNr, pageSize);

        var filteredToDos = ToDosFromRepository.Where(todo => todo.UserId == userIdFromToken);

        return filteredToDos.Select(todoEntity => _todoMapper.MapToDTO(todoEntity)).ToList();
    }


  
    public async Task<ToDoDTO?> GetByIdAsync(int userIdFromToken, int toDoId)
    {
        _logger.LogDebug($"Retrieving todo with ID {toDoId} for user {userIdFromToken}.");

        var toDoFromRepository = await _todoRepository.GetByIdAsync(toDoId);
        if (toDoFromRepository == null)
        {
            _logger.LogNotFound("todo", toDoId);
            throw ExceptionHelper.CreateNotFoundException("todo", toDoId);
        }

        if (toDoFromRepository.UserId != userIdFromToken)
        {
            _logger.LogUnauthorizedAccess("todo", toDoId, userIdFromToken);
            throw ExceptionHelper.CreateUnauthorizedException("todo", toDoId);
        }

        _logger.LogOperationSuccess("retrieved", "todo", toDoId);
        return _todoMapper.MapToDTO(toDoFromRepository);
    }


    
    public async Task<ToDoDTO?> UpdateAsync(int userIdFromToken, int toDoId, ToDoDTO todoDto)
    {
        _logger.LogDebug($"Updating todo with ID {toDoId} for user {userIdFromToken}.");


        var existingTodo = await _todoRepository.GetByIdAsync(toDoId);
        if (existingTodo == null)
        {
            _logger.LogNotFound("todo", toDoId);
            throw ExceptionHelper.CreateNotFoundException("todo", toDoId);
        }

       
        if (existingTodo.UserId != userIdFromToken)
        {
            _logger.LogUnauthorizedAccess("todo", toDoId, userIdFromToken);
            throw ExceptionHelper.CreateUnauthorizedException("todo", toDoId);
        }

        var todoToUpdate = _todoMapper.MapToModel(todoDto);
        todoToUpdate.Id = toDoId;

       
        var updatedTodo = await _todoRepository.UpdateAsync(toDoId, todoToUpdate);
        if (updatedTodo == null)
        {
            _logger.LogOperationFailure("update", "todo", toDoId);
            throw ExceptionHelper.CreateOperationException("todo", toDoId, "update");
        }

        _logger.LogOperationSuccess("updated", "todo", toDoId);
        return _todoMapper.MapToDTO(updatedTodo);
    }


   
    public async Task<ToDoDTO?> DeleteAsync(int userIdFromToken, int toDoId)
    {
        _logger.LogDebug($"Deleting todo with ID {toDoId} for user {userIdFromToken}.");


        var toDoToDelete = await _todoRepository.GetByIdAsync(toDoId);
        if (toDoToDelete == null)
        {
            _logger.LogNotFound("todo", toDoId);
            throw ExceptionHelper.CreateNotFoundException("todo", toDoId);
        }

       
        if (toDoToDelete.UserId != userIdFromToken)
        {
            _logger.LogUnauthorizedAccess("todo", toDoId, userIdFromToken);
            throw ExceptionHelper.CreateUnauthorizedException("todo", toDoId);
        }

        
        var deletedToDo = await _todoRepository.DeleteAsync(toDoId);
        if (deletedToDo == null)
        {
            _logger.LogOperationFailure("delete", "todo", toDoId);
            throw ExceptionHelper.CreateOperationException("todo", toDoId, "delete");
        }

        _logger.LogOperationSuccess("deleted", "todo", toDoId);
        return _todoMapper.MapToDTO(toDoToDelete);
    }
}