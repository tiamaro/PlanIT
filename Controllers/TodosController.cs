using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanIT.API.Extensions;
using PlanIT.API.Middleware;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Services.Interfaces;

namespace PlanIT.API.Controllers;

// TodoController - API Controller for todo management:
// - The controller handles all requests related to todos, including registration,
//   updating, deletion, and retrieval of todo information. It receives an instance of IService
//   as part of the constructor to perform operations related to todos.
//
// Policy:
// - "Bearer": Requires that all calls to this controller are authenticated with a valid JWT token
//   that meets the requirements defined in the "Bearer" authentication policy. This ensures that only
//   authenticated users can access the endpoints defined in this controller.
//
// HandleExceptionFilter:
// - This filter is attached to the controller to catch and handle exceptions in a centralized manner.
//
// Requests starting with "api/v1/Todos" will be routed to methods defined in this controller.


[Authorize(Policy = "Bearer")]
[ApiController]
[Route("api/v1/[controller]")]
[ServiceFilter(typeof(HandleExceptionFilter))]  // Uses HandleExceptionFilter to handle exceptions
public class TodosController : ControllerBase
{
    private readonly IService<ToDoDTO> _todoService;
    private readonly ILogger<TodosController> _logger;

    public TodosController(ILogger<TodosController> logger,
        IService<ToDoDTO> todoService)
    {
        _logger = logger;
        _todoService = todoService;
    }


    // Registers a new todo item
    // POST /api/v1/Todos/register
    [HttpPost("register", Name = "AddToDo")]
    public async Task<ActionResult<ToDoDTO>> AddToDoAsync(ToDoDTO newTodoDto)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        // Checks if the model state is valid after model binding and validation
        if (!ModelState.IsValid)
        {
            _logger.LogError("Invalid model state in AddToDoAsync");
            return BadRequest(ModelState);
        }

        
        var addedToDo = await _todoService.CreateAsync(userId, newTodoDto);

        // Returns new todo, or an error message if registration fails
        return addedToDo != null
            ? Ok(addedToDo)
            : BadRequest("Failed to register new todo item");
    }


    // Retrieves a paginated list of todos.
    // GET: /api/v1/Todos?pageNr=1&pageSize=10
    [HttpGet(Name = "GetToDoLists")]
    public async Task<ActionResult<IEnumerable<ToDoDTO>>> GetTodosAsync(int pageNr, int pageSize)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);


        var allToDos = await _todoService.GetAllAsync(userId, pageNr, pageSize);

        

        // Returns list of todos, or an error message if not found
        return allToDos != null
           ? Ok(allToDos)
           : NotFound("No registered todo items found.");
    }


    // Retrieves a specific todo by its ID.
    // GET /api/v1/Todos/{id}
    [HttpGet("{toDoId}", Name = "GetToDoById")]
    public async Task<ActionResult<ToDoDTO>> GetTodoByIdAsync(int toDoId)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        
        var exsistingToDo = await _todoService.GetByIdAsync(userId, toDoId);

        // Returns todo, or an error message if not found
        return exsistingToDo != null
            ? Ok(exsistingToDo)
            : NotFound("Todo item not found");
    }


    // Updates a todo based on the provided ID.
    // PUT /api/v1/Todos/{id}
    [HttpPut("{toDoId}", Name = "UpdateTodo")]
    public async Task<ActionResult<ToDoDTO>> UpdateTodoAsync(int toDoId, ToDoDTO updatedTodoDto)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        
        var updatedToDoResult = await _todoService.UpdateAsync(userId, toDoId, updatedTodoDto);

        // Returns updated todo, or an error message if update fails
        return updatedToDoResult != null
            ? Ok(updatedToDoResult)
            : NotFound("Unable to update the todo item or the todo item does not belong to the user");
    }


    // Deletes a todo based on the provided ID.
    // DELETE /api/v1/Todos/{id}
    [HttpDelete("{toDoId}", Name = "DeleteTodo")]
    public async Task<ActionResult<ToDoDTO>> DeleteTodoAsync(int toDoId)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        
        var deletedToDoResult = await _todoService.DeleteAsync(userId, toDoId);


        // Returns deleted todo, or an error message if deletion fails
        return deletedToDoResult != null
            ? Ok(deletedToDoResult)
            : NotFound("Unable to delete todo item or the todo item does not belong to the user");
    }
}
