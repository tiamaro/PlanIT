using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Services.Interfaces;
using PlanIT.API.Mappers.Interface;

namespace PlanIT.API.Services;

public class ShoppingListService : IService<ShoppingListDTO>
{
    private readonly IRepository<ShoppingList> _shoppingListRepository;
    private readonly IMapper<ShoppingList, ShoppingListDTO> _shoppingListMapper;
    private readonly ILogger<ShoppingListService> _logger;

    public ShoppingListService(
        IRepository<ShoppingList> shoppingListRepository,
        IMapper<ShoppingList, ShoppingListDTO> shoppingListMapper,
        ILogger<ShoppingListService> logger)
    {
        _shoppingListRepository = shoppingListRepository ?? throw new ArgumentNullException(nameof(shoppingListRepository));
        _shoppingListMapper = shoppingListMapper ?? throw new ArgumentNullException(nameof(shoppingListMapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ShoppingListDTO?> CreateAsync(ShoppingListDTO newDto)
    {
        return await HandleServiceCallAsync(async () =>
        {
            var shoppingList = _shoppingListMapper.MapToModel(newDto);
            var addedShoppingList = await _shoppingListRepository.AddAsync(shoppingList);

            return addedShoppingList != null ? _shoppingListMapper.MapToDTO(addedShoppingList) : null;
        }, "Failed to create or modify a shopping list.");
    }

    public async Task<ICollection<ShoppingListDTO>> GetAllAsync(int pageNr, int pageSize)
    {
        return await HandleServiceCallAsync(async () =>
        {
            var shoppingLists = await _shoppingListRepository.GetAllAsync(pageNr, pageSize);
            return shoppingLists.Select(_shoppingListMapper.MapToDTO).ToList();
        }, "Failed to retrieve shopping lists.");
    }

    public async Task<ShoppingListDTO?> GetByIdAsync(int id)
    {
        return await HandleServiceCallAsync(async () =>
        {
            var shoppingList = await _shoppingListRepository.GetByIdAsync(id);
            return shoppingList != null ? _shoppingListMapper.MapToDTO(shoppingList) : null;
        }, "Failed to retrieve a shopping list by ID.");
    }

    public async Task<ShoppingListDTO?> UpdateAsync(int id, ShoppingListDTO dto)
    {
        return await HandleServiceCallAsync(async () =>
        {
            var existingShoppingList = await _shoppingListRepository.GetByIdAsync(id);
            if (existingShoppingList == null)
                return null;

            var updatedShoppingList = _shoppingListMapper.MapToModel(dto);
            updatedShoppingList.Id = id;
            updatedShoppingList = await _shoppingListRepository.UpdateAsync(id, updatedShoppingList);

            return updatedShoppingList != null ? _shoppingListMapper.MapToDTO(updatedShoppingList) : null;
        }, "Failed to update a shopping list.");
    }

    public async Task<ShoppingListDTO?> DeleteAsync(int id)
    {
        return await HandleServiceCallAsync(async () =>
        {
            var deletedShoppingList = await _shoppingListRepository.DeleteAsync(id);
            return deletedShoppingList != null ? _shoppingListMapper.MapToDTO(deletedShoppingList) : null;
        }, "Failed to delete a shopping list.");
    }

    private async Task<T> HandleServiceCallAsync<T>(Func<Task<T>> action, string errorMessage)
    {
        try
        {
            return await action();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request: {ErrorMessage}", errorMessage);
            throw;
        }
    }
}
