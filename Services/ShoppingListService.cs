using PlanIT.API.Mappers;
using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Services.Interfaces;
using PlanIT.API.Utilities; // Inkluderer tilgang til LoggerService og ExceptionHelper

namespace PlanIT.API.Services;


// Serviceklasse for håndtering av handlelisteinformasjon.
// Exceptions blir fanget av en middleware: HandleExceptionFilter
public class ShoppingListService : IService<ShoppingListDTO>
{
    private readonly IRepository<ShoppingList> _shoppingListRepository;
    private readonly IMapper<ShoppingList, ShoppingListDTO> _shoppingListMapper;
    private readonly LoggerService _logger;

    public ShoppingListService(
        IRepository<ShoppingList> shoppingListRepository,
        IMapper<ShoppingList, ShoppingListDTO> shoppingListMapper,
        LoggerService logger)
    {
        _shoppingListRepository = shoppingListRepository;
        _shoppingListMapper = shoppingListMapper;
        _logger = logger;
    }

    // Oppretter en ny handleliste basert på data mottatt fra klienten og lagrer den i databasen.
    public async Task<ShoppingListDTO?> CreateAsync(int userIdFromToken, ShoppingListDTO newShoppingListDto)
    {
        _logger.LogCreationStart("shopping list");
        var newShoppingList = _shoppingListMapper.MapToModel(newShoppingListDto);

        newShoppingList.UserId = userIdFromToken;

        // Forsøker å legge til den nye handlelisten i databasen
        var addedShoppingList = await _shoppingListRepository.AddAsync(newShoppingList);
        if (addedShoppingList == null)
        {
            _logger.LogCreationFailure("shopping list");
            throw ExceptionHelper.CreateOperationException("shopping list", 0, "create");
        }

        // Logger at handleliseten ble vellykket opprettet med tilhørende ID
        _logger.LogOperationSuccess("created", "shopping list", addedShoppingList.Id);
        return _shoppingListMapper.MapToDTO(addedShoppingList);
    }


    // Henter alle handlelister for innlogget bruker fra databasen med støtte for paginering.
    public async Task<ICollection<ShoppingListDTO>> GetAllAsync(int userIdFromToken, int pageNr, int pageSize)
    {
        var shoppingListsFromRepository = await _shoppingListRepository.GetAllAsync(1, 10);
        var filteredShoppingList = shoppingListsFromRepository.Where(shopping => shopping.UserId == userIdFromToken);

        return filteredShoppingList.Select(shoppingEntity => _shoppingListMapper.MapToDTO(shoppingEntity)).ToList();
    }



    // Henter en spesifikk handleliste basert på ID og sjekker at brukeren har tilgang.
    public async Task<ShoppingListDTO?> GetByIdAsync(int userIdFromToken, int shoppingListId)
    {
        _logger.LogDebug("Attempting to retrieve shopping list with ID {ShoppingListId} for user ID {UserId}.", shoppingListId, userIdFromToken);

        // Henter handleliste basert på ID
        var shoppingListFromRepository = await _shoppingListRepository.GetByIdAsync(shoppingListId);
        if (shoppingListFromRepository == null)
        {
            _logger.LogNotFound("shopping list", shoppingListId);
            throw ExceptionHelper.CreateNotFoundException("shopping list", shoppingListId);
        }

        // Sjekker om brukerens ID stemmer overens med brukerID tilknyttet handlelisten
        if (shoppingListFromRepository.UserId != userIdFromToken)
        {
            _logger.LogUnauthorizedAccess("shopping list", shoppingListId, userIdFromToken);
            throw ExceptionHelper.CreateUnauthorizedException("shopping list", shoppingListId);
        }

        _logger.LogOperationSuccess("retrieved", "shopping list", shoppingListId);
        return _shoppingListMapper.MapToDTO(shoppingListFromRepository);
    }


    // Oppdaterer en eksisterende handleliste basert på data mottatt fra klienten.
    public async Task<ShoppingListDTO?> UpdateAsync(int userIdFromToken, int shoppingListId, ShoppingListDTO shoppingListDTO)
    {
        _logger.LogDebug("Attempting to update shopping list with ID {ShoppingListId} by user ID {UserId}.", shoppingListId, userIdFromToken);

        // Forsøker å hente et handleliste basert på ID for å sikre at det faktisk eksisterer før oppdatering.
        var existingShoppingList = await _shoppingListRepository.GetByIdAsync(shoppingListId);
        if (existingShoppingList == null)
        {
            _logger.LogNotFound("shopping list", shoppingListId);
            throw ExceptionHelper.CreateNotFoundException("shopping list", shoppingListId);
        }

        // Sjekker om brukeren som prøver å oppdatere handlelisten er den samme brukeren som opprettet den.
        if (existingShoppingList.UserId != userIdFromToken)
        {
            _logger.LogUnauthorizedAccess("shopping list", shoppingListId, userIdFromToken);
            throw ExceptionHelper.CreateUnauthorizedException("shopping list", shoppingListId);
        }

        var shoppingListToUpdate = _shoppingListMapper.MapToModel(shoppingListDTO);
        shoppingListToUpdate.Id = shoppingListId; // Sikrer at ID ikke endres

        // Utfører oppdateringen av handlelisten fra databasen.
        var updatedShoppingList = await _shoppingListRepository.UpdateAsync(shoppingListId, shoppingListToUpdate);
        if (updatedShoppingList == null)
        {
            _logger.LogOperationFailure("update", "shopping list", shoppingListId);
            throw ExceptionHelper.CreateOperationException("shopping list", shoppingListId, "update");
        }

        _logger.LogOperationSuccess("updated", "shopping list", shoppingListId);
        return _shoppingListMapper.MapToDTO(updatedShoppingList);
    }


    // Sletter en handleliste basert på ID etter å ha sjekket at brukeren har tilgang.
    public async Task<ShoppingListDTO?> DeleteAsync(int userIdFromToken, int shoppingListId)
    {
        _logger.LogDebug("Attempting to delete shopping list with ID {ShoppingListId} by user ID {UserId}.", shoppingListId, userIdFromToken);

        // Forsøker å hente et handleliste basert på ID for å sikre at det faktisk eksisterer før sletting.
        var existingShoppingList = await _shoppingListRepository.GetByIdAsync(shoppingListId);
        if (existingShoppingList == null)
        {
            _logger.LogNotFound("shopping list", shoppingListId);
            throw ExceptionHelper.CreateNotFoundException("shopping list", shoppingListId);
        }

        // Sjekker om brukeren som prøver å slette handlelisten er den samme brukeren som opprettet den.
        if (existingShoppingList.UserId != userIdFromToken)
        {
            _logger.LogUnauthorizedAccess("shopping list", shoppingListId, userIdFromToken);
            throw ExceptionHelper.CreateUnauthorizedException("shopping list", shoppingListId);
        }

        // Utfører sletting av handlelisten fra databasen.
        var deletedShoppingList = await _shoppingListRepository.DeleteAsync(shoppingListId);
        if (deletedShoppingList == null)
        {
            _logger.LogOperationFailure("delete", "shopping list", shoppingListId);
            throw ExceptionHelper.CreateOperationException("shopping list", shoppingListId, "delete");
        }

        _logger.LogOperationSuccess("deleted", "shopping list", shoppingListId);
        return _shoppingListMapper.MapToDTO(deletedShoppingList);
    }
}