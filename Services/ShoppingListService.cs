using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Services.Interfaces;

namespace PlanIT.API.Services;


// Serviceklasse for håndtering av handlelisteinformasjon.
// Exceptions blir fanget av en middleware: HandleExceptionFilter
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
        _shoppingListRepository = shoppingListRepository;
        _shoppingListMapper = shoppingListMapper;
        _logger = logger;
    }


    // Oppretter ny handleliste
    public async Task<ShoppingListDTO?> CreateAsync(ShoppingListDTO newShoppingListDto)
    {
        _logger.LogInformation("Starting to create a new shoppinglist.");

        var newShoppingList = _shoppingListMapper.MapToModel(newShoppingListDto);

        // Legger til den nye handlelisten i databasen og henter resultatet
        var addedShoppingList = await _shoppingListRepository.AddAsync(newShoppingList);
        if (addedShoppingList == null)
        {
            _logger.LogWarning("Failed to create new shoppinglist.");
            throw new InvalidOperationException("The shoppinglist could not be created due to invalid data or state.");
        }

        _logger.LogInformation("New shoppinglist created successfully with ID {shoppingListId}.", addedShoppingList.Id);
        return _shoppingListMapper.MapToDTO(addedShoppingList);
    }

    // *********NB!! FJERNE??? *************
    //
    // Henter alle handlelister med paginering
    public async Task<ICollection<ShoppingListDTO>> GetAllAsync(int pageNr, int pageSize)
    {
        var shoppingListsFromRepository = await _shoppingListRepository.GetAllAsync(1, 10);

        // Mapper til eventDTO-format
        var shoppingListDTOs = shoppingListsFromRepository.Select(shoppingListEntity => _shoppingListMapper.MapToDTO(shoppingListEntity)).ToList();
        return shoppingListDTOs;
    }


    // Henter ShoppingList basert på shoppingListId og brukerID
    public async Task<ShoppingListDTO?> GetByIdAsync(int userIdFromToken, int shoppingListId)
    {
        _logger.LogDebug("Attempting to retrieve shopping list with ID {ShoppingListId} for user ID {UserId}.", shoppingListId, userIdFromToken);

        var shoppingListFromRepository = await _shoppingListRepository.GetByIdAsync(shoppingListId);

        // Sjekker om handlelisten eksisterer
        if (shoppingListFromRepository == null)
        {
            _logger.LogWarning("Shopping list with ID {ShoppingListId} not found.", shoppingListId);
            throw new KeyNotFoundException($"Shopping list with ID {shoppingListId} not found.");
        }

        // Sjekker om brukeren er autorisert
        if (shoppingListFromRepository.UserId != userIdFromToken)
        {
            _logger.LogWarning("Unauthorized attempt to access shopping list with ID {ShoppingListId} by user ID {UserId}.", shoppingListId, userIdFromToken);
            throw new UnauthorizedAccessException($"User ID {userIdFromToken} is not authorized to access shopping list ID {shoppingListId}.");
        }

        _logger.LogInformation("Shopping list with ID {ShoppingListId} retrieved successfully for user ID {UserId}.", shoppingListId, userIdFromToken);
        return _shoppingListMapper.MapToDTO(shoppingListFromRepository);
    }


    // Oppdaterer handleliste
    public async Task<ShoppingListDTO?> UpdateAsync(int userIdFromToken, int shoppingListId, ShoppingListDTO shoppingListDTO)
    {
        _logger.LogDebug("Attempting to update shopping list with ID {ShoppingListId} by user ID {UserId}.", shoppingListId, userIdFromToken);

        var existingShoppingList = await _shoppingListRepository.GetByIdAsync(shoppingListId);
        if (existingShoppingList == null)
        {
            _logger.LogWarning("Shopping list with ID {ShoppingListId} not found.", shoppingListId);
            throw new KeyNotFoundException($"Shopping list with ID {shoppingListId} not found.");
        }

        if (existingShoppingList.UserId != userIdFromToken)
        {
            _logger.LogWarning("Unauthorized update attempt by User ID {UserId} on shopping list ID {ShoppingListId}.", userIdFromToken, shoppingListId);
            throw new UnauthorizedAccessException($"User ID {userIdFromToken} is not authorized to update shopping list ID {shoppingListId}.");
        }

        // Mapper og forsikrer at ID blir det samme under opopdatering
        var shoppingListToUpdate = _shoppingListMapper.MapToModel(shoppingListDTO);
        shoppingListToUpdate.Id = shoppingListId;

        // Prøver å oppdatere handlelisten
        var updatedShoppingList = await _shoppingListRepository.UpdateAsync(shoppingListId, shoppingListToUpdate);
        if (updatedShoppingList == null)
        {
            _logger.LogError("Failed to update shopping list with ID {ShoppingListId}.", shoppingListId);
            throw new InvalidOperationException($"Failed to update shopping list with ID {shoppingListId}.");
        }

        _logger.LogInformation("Shopping list with ID {ShoppingListId} updated successfully.", shoppingListId);
        return _shoppingListMapper.MapToDTO(updatedShoppingList);
    }


    // Sletter handleliste
    public async Task<ShoppingListDTO?> DeleteAsync(int userIdFromToken, int shoppingListId)
    {
        _logger.LogDebug("Attempting to delete shopping list with ID {ShoppingListId} by user ID {UserId}.", shoppingListId, userIdFromToken);

        // Henter handleliste for å sjekke om den eksisterer og hvem som eier den
        var existingShoppingList = await _shoppingListRepository.GetByIdAsync(shoppingListId);
        if (existingShoppingList == null)
        {
            _logger.LogWarning("Shopping list with ID {ShoppingListId} not found.", shoppingListId);
            throw new KeyNotFoundException($"Shopping list with ID {shoppingListId} not found.");
        }

        if (existingShoppingList.UserId != userIdFromToken)
        {
            _logger.LogWarning("Unauthorized delete attempt by User ID {UserId} on shopping list ID {ShoppingListId}.", userIdFromToken, shoppingListId);
            throw new UnauthorizedAccessException($"User ID {userIdFromToken} is not authorized to delete shopping list ID {shoppingListId}.");
        }

        // Prøver å slette handlelisten
        var deletedShoppingList = await _shoppingListRepository.DeleteAsync(shoppingListId);
        if (deletedShoppingList == null)
        {
            _logger.LogError("Failed to delete shopping list with ID {ShoppingListId}.", shoppingListId);
            throw new InvalidOperationException($"Failed to delete shopping list with ID {shoppingListId}.");
        }

        _logger.LogInformation("Shopping list with ID {ShoppingListId} deleted successfully.", shoppingListId);
        return _shoppingListMapper.MapToDTO(deletedShoppingList);
    }
}
