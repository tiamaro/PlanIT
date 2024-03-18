using Microsoft.AspNetCore.Mvc;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Services.Interfaces;

namespace PlanIT.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ShoppingListController : ControllerBase
{
    private readonly ILogger<ShoppingListController> _logger;
    private readonly IService<ShoppingListDTO> _shoppingListService;

    public ShoppingListController(ILogger<ShoppingListController> logger, IService<ShoppingListDTO> shoppingListService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _shoppingListService = shoppingListService ?? throw new ArgumentNullException(nameof(shoppingListService));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] ShoppingListDTO newDto)
    {
        return await HandleRequestAsync(async () =>
        {
            var createdShoppingList = await _shoppingListService.CreateAsync(newDto);
            if (createdShoppingList != null)
            {
                return CreatedAtRoute("GetById", new { id = createdShoppingList.Id }, createdShoppingList);
            }
            else
            {
                _logger.LogError("Failed to create a new shopping list.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        return await HandleRequestAsync(async () =>
        {
            var shoppingList = await _shoppingListService.GetByIdAsync(id);
            return shoppingList != null ? Ok(shoppingList) : NotFound();
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(int pageNr, int pageSize)
    {
        return await HandleRequestAsync(async () =>
        {
            var shoppingLists = await _shoppingListService.GetAllAsync(pageNr, pageSize);
            return Ok(shoppingLists);
        });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] ShoppingListDTO dto)
    {
        return await HandleRequestAsync(async () =>
        {
            var updatedShoppingList = await _shoppingListService.UpdateAsync(id, dto);
            return updatedShoppingList == null ? NotFound() : Ok(updatedShoppingList);
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        return await HandleRequestAsync(async () =>
        {
            var deletedShoppingList = await _shoppingListService.DeleteAsync(id);
            return deletedShoppingList == null ? NotFound() : Ok(deletedShoppingList);
        });
    }

    private async Task<IActionResult> HandleRequestAsync(Func<Task<IActionResult>> action)
    {
        try
        {
            return await action.Invoke();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request.");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
