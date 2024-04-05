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
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _todoService = todoService ?? throw new ArgumentNullException(nameof(todoService));
    }

    [HttpPost]
    public async Task<ActionResult<ToDoDTO>> CreateTodoAsync(ToDoDTO newTodoDto)
    {
        try
        {
            var createdTodo = await _todoService.CreateAsync(newTodoDto);
            if (createdTodo == null)
            {
                return StatusCode(500, "Failed to create todo");
            }

            return Ok(createdTodo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create todo");
            return StatusCode(500, "An error occurred while creating todo");
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ToDoDTO>> GetTodoByIdAsync(int id)
    {
        try
        {
            var todo = await _todoService.GetByIdAsync(id);
            if (todo == null)
            {
                return NotFound();
            }
            return Ok(todo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to get todo with ID {id}");
            return StatusCode(500, $"An error occurred while getting todo with ID {id}");
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ToDoDTO>>> GetTodosAsync([FromQuery] int pageNr, [FromQuery] int pageSize)
    {
        try
        {
            if (pageNr <= 0 || pageSize <= 0)
            {
                return BadRequest("Invalid page number or page size");
            }

            var todos = await _todoService.GetAllAsync(pageNr, pageSize);
            return Ok(todos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve todos");
            return StatusCode(500, "An error occurred while retrieving todos");
        }
    }

    [HttpPut("{id:int}")]
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

    [HttpDelete("{id:int}")]
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
