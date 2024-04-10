using Microsoft.AspNetCore.Mvc;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Services.Interfaces;

namespace PlanIT.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TodoController : ControllerBase
{
    private readonly ILogger<TodoController> _logger;
    private readonly IService<ToDoDTO> _todoService;

    public TodoController(ILogger<TodoController> logger, IService<ToDoDTO> todoService)
    {
        _logger = logger;
        _todoService = todoService;
    }

    [HttpPost(Name = "AddToDo")]
    public async Task<ActionResult<ToDoDTO>> AddToDoAsync(ToDoDTO newTodoDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state in AddToDoAsync");
                return BadRequest(ModelState);
            }

            var addedToDo = await _todoService.CreateAsync(newTodoDto);
            return addedToDo != null
                ? Ok(addedToDo) 
                : BadRequest("Failed to register new event");
        }
        catch (Exception ex)
        {
            _logger.LogError("An unknown error occured: " + ex.Message);
            return StatusCode(500, "An unknown error occured, please try again later");
        }
        
    }

    [HttpGet("{toDoId}", Name = "GetToDoById")]
    public async Task<ActionResult<ToDoDTO>> GetTodoByIdAsync(int toDoId)
    {
        var exsistingToDo = await _todoService.GetByIdAsync(toDoId);

        return exsistingToDo != null
            ? Ok(exsistingToDo)
            : NotFound("Event not found");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ToDoDTO>>> GetTodosAsync(int pageNr, int pageSize)
    {
        var allToDos = await _todoService.GetAllAsync(pageNr, pageSize);

        return allToDos != null
           ? Ok(allToDos)
           : NotFound("No registered toDos found.");
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ToDoDTO>> UpdateTodoAsync(int id, ToDoDTO updatedTodoDto)
    {
        try
        {
            var updatedTodo = await _todoService.UpdateAsync(id, updatedTodoDto);
            if (updatedTodo == null)
            {
                return NotFound();
            }
            return Ok(updatedTodo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to update todo with ID {id}");
            return StatusCode(500, $"An error occurred while updating todo with ID {id}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ToDoDTO>> DeleteTodoAsync(int id)
    {
        try
        {
            var deletedTodo = await _todoService.DeleteAsync(id);
            if (deletedTodo == null)
            {
                return NotFound();
            }
            return Ok(deletedTodo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to delete todo with ID {id}");
            return StatusCode(500, $"An error occurred while deleting todo with ID {id}");
        }
    }
}
