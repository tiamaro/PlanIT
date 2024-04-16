using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Services.Interfaces;

namespace PlanIT.API.Services;

// Serviceklasse for håndtering av gjøremålsinformasjon.
// Exceptions blir fanget av en middleware: HandleExceptionFilter
public class TodoService : IService<ToDoDTO>
{
    private readonly IRepository<ToDo> _todoRepository;
    private readonly IMapper<ToDo, ToDoDTO> _todoMapper;
    private readonly ILogger<TodoService> _logger;

    public TodoService(
        IRepository<ToDo> todoRepository,
        IMapper<ToDo, ToDoDTO> todoMapper,
        ILogger<TodoService> logger)
    {
        _todoRepository = todoRepository;
        _todoMapper = todoMapper;
        _logger = logger;
    }

    // Oppretter nytt gjøremål
    public async Task<ToDoDTO?> CreateAsync(ToDoDTO newToDoDTO)
    {
        _logger.LogInformation("Starting to create a new todo item.");

        var newToDo = _todoMapper.MapToModel(newToDoDTO);

        // Legger til det nye gjøremålet i databasen og henter resultatet
        var addedToDo = await _todoRepository.AddAsync(newToDo);
        if (addedToDo == null)
        {
            _logger.LogWarning("Failed to create new todo item.");
            throw new InvalidOperationException("The todo item could not be created due to invalid data or state.");
        }

        _logger.LogInformation("New todo item created successfully with ID {ToDoId}.", addedToDo.Id);
        return _todoMapper.MapToDTO(addedToDo);
    }


    // *********NB!! FJERNE??? *************
    //
    // Henter alle gjøremål med paginering
    public async Task<ICollection<ToDoDTO>> GetAllAsync(int pageNr, int pageSize)
    {
        var toDosFromRepository = await _todoRepository.GetAllAsync(pageNr, pageSize);
        var todDoDTOs = toDosFromRepository.Select(todoEntity => _todoMapper.MapToDTO(todoEntity)).ToList();
        return todDoDTOs;
    }


    // Henter gjøremål basert på toDoID og brukerID
    public async Task<ToDoDTO?> GetByIdAsync(int userIdFromToken, int toDoId)
    {
        _logger.LogDebug("Attempting to retrieve Todo item with ID {ToDoId} for user ID {UserId}.", toDoId, userIdFromToken);

        var toDoFromRepository = await _todoRepository.GetByIdAsync(toDoId);

        // Sjekker om gjøremålet eksisterer
        if (toDoFromRepository == null)
        {
            _logger.LogWarning("Todo itemn with ID {ToDoId} not found.", toDoId);
            throw new KeyNotFoundException($"Todo item with ID {toDoId} not found.");
        }

        // Sjekker om brukeren er autorisert til å få tilgang til gjøremålet
        if (toDoFromRepository.UserId != userIdFromToken)
        {
            _logger.LogWarning("Unauthorized attempt to access Todo item with ID {ToDoId} by user ID {UserId}.", toDoId, userIdFromToken);
            throw new UnauthorizedAccessException($"User ID {userIdFromToken} is not authorized to access ToDoID {toDoId}.");
        }

        _logger.LogInformation("Todo item with ID {ToDoId} retrieved successfully for user ID {UserId}.", toDoId, userIdFromToken);
        return _todoMapper.MapToDTO(toDoFromRepository);
    }


    // Oppdaterer gjøremål
    public async Task<ToDoDTO?> UpdateAsync(int userIdFromToken, int toDoId, ToDoDTO todoDto)
    {
        _logger.LogDebug("Attempting to update Todo item with ID {ToDoId} by user ID {UserId}.", toDoId, userIdFromToken);

        var existingTodo = await _todoRepository.GetByIdAsync(toDoId);
        if (existingTodo == null)
        {
            _logger.LogWarning("Todo item with ID {ToDoId} not found.", toDoId);
            throw new KeyNotFoundException($"Todo item with ID {toDoId} not found.");
        }

        // Sjekker om brukeren er autorisert til å få tilgang til gjøremålet
        if (existingTodo.UserId != userIdFromToken)
        {
            _logger.LogWarning("Unauthorized update attempt by User ID {UserId} on ToDoID {ToDoId}.", userIdFromToken, toDoId);
            throw new UnauthorizedAccessException($"User ID {userIdFromToken} is not authorized to update ToDoID {toDoId}.");
        }

        // Mapper og passer på at ID blir uforandret under oppdatering
        var todoToUpdate = _todoMapper.MapToModel(todoDto);
        todoToUpdate.Id = toDoId;

        // Prøver å oppdatere gjøremålet
        var updatedTodo = await _todoRepository.UpdateAsync(toDoId, todoToUpdate);
        if (updatedTodo == null)
        {
            _logger.LogError("Failed to update Todo item with ID {ToDoId}.", toDoId);
            throw new InvalidOperationException($"Failed to update Todo item with ID {toDoId}.");
        }

        _logger.LogInformation("Todo item with ID {ToDoId} updated successfully.", toDoId);
        return _todoMapper.MapToDTO(updatedTodo);
    }



    // Sletter gjøremålet
    public async Task<ToDoDTO?> DeleteAsync(int userIdFromToken, int toDoId)
    {
        _logger.LogDebug("Attempting to delete Todo item with ID {ToDoId} by user ID {UserId}.", toDoId, userIdFromToken);

        var toDoToDelete = await _todoRepository.GetByIdAsync(toDoId);
        if (toDoToDelete == null)
        {
            _logger.LogWarning("Todo item with ID {ToDoId} not found.", toDoId);
            throw new KeyNotFoundException($"Todo item with ID {toDoId} not found.");
        }

        // Sjekker om brukeren er autorisert til å få tilgang til gjøremålet
        if (toDoToDelete.UserId != userIdFromToken)
        {
            _logger.LogWarning("Unauthorized delete attempt by User ID {UserId} on ToDoID {ToDoId}.", userIdFromToken, toDoId);
            throw new UnauthorizedAccessException($"User ID {userIdFromToken} is not authorized to delete ToDoID {toDoId}.");
        }

        // Prøver å slette gjøremålet
        var deletedToDo = await _todoRepository.DeleteAsync(toDoId);
        if (deletedToDo == null)
        {
            _logger.LogError("Failed to delete Todo item with ID {ToDoId}.", toDoId);
            throw new InvalidOperationException($"Failed to delete Todo item with ID {toDoId}.");
        }

        _logger.LogInformation("Todo item with ID {ToDoId} deleted successfully.", toDoId);
        return _todoMapper.MapToDTO(toDoToDelete);
    }
}
