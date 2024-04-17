using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Services.Interfaces;
using PlanIT.API.Utilities; // Inkluderer tilgang til LoggerService og ExceptionHelper

namespace PlanIT.API.Services;

// Serviceklasse for håndtering av gjøremålsinformasjon.
// Exceptions blir fanget av en middleware: HandleExceptionFilter
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

    // // Oppretter et nytt gjøremål basert på data mottatt fra klienten
    public async Task<ToDoDTO?> CreateAsync(ToDoDTO newToDoDTO)
    {
        _logger.LogCreationStart("todo");

        var newToDo = _todoMapper.MapToModel(newToDoDTO);
        var addedToDo = await _todoRepository.AddAsync(newToDo);
        if (addedToDo == null)
        {
            _logger.LogCreationFailure("todo");
            throw ExceptionHelper.CreateOperationException("todo", 0, "create");
        }

        _logger.LogOperationSuccess("created", "todo", addedToDo.Id);
        return _todoMapper.MapToDTO(addedToDo);
    }


    // ''''''''''''''''''''''''' FJERNE ????????? ''''''''''''''''''''''''
    //
    // Henter alle gjøremål med paginering
    public async Task<ICollection<ToDoDTO>> GetAllAsync(int pageNr, int pageSize)
    {
        var toDosFromRepository = await _todoRepository.GetAllAsync(pageNr, pageSize);
        return toDosFromRepository.Select(todoEntity => _todoMapper.MapToDTO(todoEntity)).ToList();
    }


    // Henter et spesifikt gjøremål basert på ID og validerer brukerens tilgang
    public async Task<ToDoDTO?> GetByIdAsync(int userIdFromToken, int toDoId)
    {
        _logger.LogDebug("Attempting to retrieve Todo item with ID {ToDoId} for user ID {UserId}.", toDoId, userIdFromToken);

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


    // Oppdaterer et eksisterende gjøremål etter å ha validert brukerens autorisasjon
    public async Task<ToDoDTO?> UpdateAsync(int userIdFromToken, int toDoId, ToDoDTO todoDto)
    {
        _logger.LogDebug("Attempting to update Todo item with ID {ToDoId} by user ID {UserId}.", toDoId, userIdFromToken);

        // Forsøker å hente et gjøremål basert på ID for å sikre at det faktisk eksisterer før oppdatering.
        var existingTodo = await _todoRepository.GetByIdAsync(toDoId);
        if (existingTodo == null)
        {
            _logger.LogNotFound("todo", toDoId);
            throw ExceptionHelper.CreateNotFoundException("todo", toDoId);
        }

        // Sjekker om brukeren som prøver å oppdatere gjøremålet er den samme brukeren som opprettet det.
        if (existingTodo.UserId != userIdFromToken)
        {
            _logger.LogUnauthorizedAccess("todo", toDoId, userIdFromToken);
            throw ExceptionHelper.CreateUnauthorizedException("todo", toDoId);
        }

        var todoToUpdate = _todoMapper.MapToModel(todoDto);
        todoToUpdate.Id = toDoId;

        // Utfører oppdateringen av gjøremålet fra databasen.
        var updatedTodo = await _todoRepository.UpdateAsync(toDoId, todoToUpdate);
        if (updatedTodo == null)
        {
            _logger.LogOperationFailure("update", "todo", toDoId);
            throw ExceptionHelper.CreateOperationException("todo", toDoId, "update");
        }

        _logger.LogOperationSuccess("updated", "todo", toDoId);
        return _todoMapper.MapToDTO(updatedTodo);
    }


    // Sletter et gjøremål etter å ha sjekket at brukeren er autorisert
    public async Task<ToDoDTO?> DeleteAsync(int userIdFromToken, int toDoId)
    {
        _logger.LogDebug("Attempting to delete Todo item with ID {ToDoId} by user ID {UserId}.", toDoId, userIdFromToken);

        // Forsøker å hente et gjøremål basert på ID for å sikre at det faktisk eksisterer før sletting.
        var toDoToDelete = await _todoRepository.GetByIdAsync(toDoId);
        if (toDoToDelete == null)
        {
            _logger.LogNotFound("todo", toDoId);
            throw ExceptionHelper.CreateNotFoundException("todo", toDoId);
        }

        // Sjekker om brukeren som prøver å slette gjøremålet er den samme brukeren som opprettet det.
        if (toDoToDelete.UserId != userIdFromToken)
        {
            _logger.LogUnauthorizedAccess("todo", toDoId, userIdFromToken);
            throw ExceptionHelper.CreateUnauthorizedException("todo", toDoId);
        }

        // Utfører slettingen av gjøremålet fra databasen.
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