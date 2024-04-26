using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanIT.API.Extensions;
using PlanIT.API.Middleware;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Services.Interfaces;


namespace PlanIT.API.Controllers;

// ShoppingListController - API Controller for shopping list management:
// - The controller handles all requests related to shopping list data, including registration,
//   updating, deletion, and retrieval of shopping list information. It receives an instance of IService
//   as part of the constructor to perform operations related to the shopping list.
//
// Policy:
// - "Bearer": Requires that all calls to this controller are authenticated with a valid JWT token
//   that meets the requirements defined in the "Bearer" authentication policy. This ensures that only
//   authenticated users can access the endpoints defined in this controller.
//
// HandleExceptionFilter:
// - This filter is attached to the controller to catch and handle exceptions in a centralized manner.
//
// Requests starting with "api/v1/ShoppingLists" will be routed to methods defined in this controller.


[Authorize(Policy = "Bearer")]
[ApiController]
[Route("api/v1/[controller]")]
[ServiceFilter(typeof(HandleExceptionFilter))]  // Uses HandleExceptionFilter to handle exceptions
public class ShoppingListsController : ControllerBase
{
    private readonly ILogger<ShoppingListsController> _logger;
    private readonly IService<ShoppingListDTO> _shoppingListService;

    public ShoppingListsController(ILogger<ShoppingListsController> logger,
        IService<ShoppingListDTO> shoppingListService)
    {
        _logger = logger;
        _shoppingListService = shoppingListService;
    }

    // Registers a new shoppinglist item
    // POST /api/v1/ShoppinLists/register
    [HttpPost("register", Name = "AddShoppingList")]
    public async Task<IActionResult> AddShoppingListAsync(ShoppingListDTO newShoppingListDto)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        // Checks if the model state is valid after model binding and validation
        if (!ModelState.IsValid)
        {
            _logger.LogError("Invalid model state in AddEventAsync");
            return BadRequest(ModelState);
        }

        
        var addedShoppingList = await _shoppingListService.CreateAsync(userId, newShoppingListDto);

        // Returns new shoppinglist item, or an error message if registration fails
        return addedShoppingList != null
            ? Ok(addedShoppingList)
            : BadRequest("Failed to register new shoppingList");
    }


    // Retrieves a paginated list of shoppinglist items.
    // GET: /api/v1/ShoppingLists?pageNr=1&pageSize=10
    [HttpGet(Name = "GetShoppingLists")]
    public async Task<ActionResult<IEnumerable<ShoppingListDTO>>> GetShoppingListsAsync(int pageNr, int pageSize)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);


        var allShoppingLists = await _shoppingListService.GetAllAsync(userId, pageNr, pageSize);


        return allShoppingLists != null
            ? Ok(allShoppingLists)
            : NotFound("No registered shoppingList found.");
    }


    // Retrieves a specific shoppinglist item by its ID.
    // GET /api/v1/ShoppingLists/{id}
    [HttpGet("{shoppingListId}", Name = "GetShoppingListById")]
    public async Task<ActionResult<ShoppingListDTO>> GetShoppingListByIdAsync(int shoppingListId)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        
        var existingShoppingList = await _shoppingListService.GetByIdAsync(userId, shoppingListId);

        // Returns shoppinglist item, or an error message if not found
        return existingShoppingList != null
            ? Ok(existingShoppingList)
            : NotFound("ShoppingList not found");
    }


    // Updates a shoppinglist item based on the provided ID.
    // PUT /api/v1/ShoppingLists/{id}
    [HttpPut("{shoppingListId}", Name = "UpdateShoppingList")]
    public async Task<ActionResult<ShoppingListDTO>> UpdateShoppingListAsync(int shoppingListId, ShoppingListDTO updatedShoppingListDTO)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        
        var updatedShoppingListResult = await _shoppingListService.UpdateAsync(userId, shoppingListId, updatedShoppingListDTO);

        // Returns updated shoppinglist item, or an error message if update fails
        return updatedShoppingListResult != null
            ? Ok(updatedShoppingListResult)
            : NotFound("Unable to update the shoppingList or the shoppingList does not belong to the user");
    }


    // Deletes a shoppinglist item based on the provided ID.
    // DELETE /api/v1/ShoppingLists/{id}
    [HttpDelete("{shoppingListId}", Name = "DeleteShoppingList")]
    public async Task<ActionResult<ShoppingListDTO>> DeleteShoppingListAsync(int shoppingListId)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        
        var deletedShoppingListResult = await _shoppingListService.DeleteAsync(userId, shoppingListId);

        // Returns deleted shoppinglist item, or an error message if deletion fails
        return deletedShoppingListResult != null
            ? Ok(deletedShoppingListResult)
            : NotFound("Unable to delete shoppinglist or shoppinglist does not belong to the user");
    }
}