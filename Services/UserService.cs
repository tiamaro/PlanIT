using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Services.Interfaces;
using PlanIT.API.Utilities; // Inkluderer tilgang til LoggerService og ExceptionHelper

namespace PlanIT.API.Services;

// Serviceklasse for håndtering av brukerinformasjon.
// Exceptions blir fanget av en middleware: HandleExceptionFilter
public class UserService : IUserService
{
    private readonly IMapper<User, UserDTO> _userMapper;
    private readonly IMapper<User, UserRegDTO> _userRegMapper;
    private readonly IUserRepository _userRepository;
    private readonly LoggerService _logger;

    public UserService(IMapper<User, UserDTO> userMapper,
        IMapper<User, UserRegDTO> userRegMapper,
        IUserRepository userRepository,
        LoggerService logger) 
    {
        _userMapper = userMapper;
        _userRegMapper = userRegMapper;
        _userRepository = userRepository;
        _logger = logger;
    }

    // Registrerer en ny bruker i systemet og returnerer brukerdata hvis vellykket.
    public async Task<UserDTO?> RegisterUserAsync(UserRegDTO userRegDTO)
    {
        _logger.LogCreationStart("user");

        var newUser = _userRegMapper.MapToModel(userRegDTO);
        newUser.Salt = BCrypt.Net.BCrypt.GenerateSalt();
        newUser.HashedPassword = BCrypt.Net.BCrypt.HashPassword(userRegDTO.Password, newUser.Salt);

        var addedUser = await _userRepository.AddAsync(newUser);
        if (addedUser == null)
        {
            _logger.LogCreationFailure("user");
            throw ExceptionHelper.CreateOperationException("user", newUser.Id, "register");
        }

        _logger.LogOperationSuccess("registered", "user", addedUser.Id);
        return _userMapper.MapToDTO(addedUser);
    }

    // Henter en liste over alle brukere, støtter paginering.
    public async Task<ICollection<UserDTO>> GetAllAsync(int pageNr, int pageSize)
    {
        _logger.LogInfo($"Fetching all users with pagination: Page {pageNr}, Size {pageSize}");

        var usersFromRepository = await _userRepository.GetAllAsync(pageNr, pageSize);
        if (!usersFromRepository.Any())
        {
            _logger.LogWarning("No users found during pagination retrieval.");
            return new List<UserDTO>();
        }

        var userDTOs = usersFromRepository.Select(user => _userMapper.MapToDTO(user)).ToList();
        return userDTOs;
    }

    // Henter en spesifikk bruker basert på brukerens ID og sjekker tilgang.
    public async Task<UserDTO?> GetByIdAsync(int userId)
    {
        _logger.LogDebug($"Attempting to retrieve user with ID: {userId}");

        var userFromRepository = await _userRepository.GetByIdAsync(userId);
        if (userFromRepository == null)
        {
            _logger.LogNotFound("user", userId);
            throw ExceptionHelper.CreateNotFoundException("user", userId);
        }

        return _userMapper.MapToDTO(userFromRepository);
    }

    // Oppdaterer brukerinformasjonen for en eksisterende bruker og sjekker tilgangsrettigheter.
    public async Task<UserDTO?> UpdateAsync(int userId, UserDTO userDTO)
    {
        _logger.LogDebug($"Attempting to update user with ID: {userId}");

        var existingUser = await _userRepository.GetByIdAsync(userId);
        if (existingUser == null)
        {
            _logger.LogNotFound("user", userId);
            throw ExceptionHelper.CreateNotFoundException("user", userId);
        }

        if (existingUser.Id != userId)
        {
            _logger.LogUnauthorizedAccess("user", userId, userId);
            throw ExceptionHelper.CreateUnauthorizedException("user", userId);
        }

        var userToUpdate = _userMapper.MapToModel(userDTO);
        var updatedUser = await _userRepository.UpdateAsync(userId, userToUpdate);
        if (updatedUser == null)
        {
            _logger.LogOperationFailure("update", "user", userId);
            throw ExceptionHelper.CreateOperationException("user", userId, "update");
        }

        _logger.LogOperationSuccess("updated", "user", userId);
        return _userMapper.MapToDTO(updatedUser);
    }


    // Sletter en bruker basert på brukerens ID og utfører sjekker for tilgang.
    public async Task<UserDTO?> DeleteAsync(int userId)
    {
        _logger.LogDebug($"Attempting to delete user with ID: {userId}");

        var userToDelete = await _userRepository.GetByIdAsync(userId);
        if (userToDelete == null)
        {
            _logger.LogNotFound("user", userId);
            throw ExceptionHelper.CreateNotFoundException("user", userId);
        }

        if (userToDelete.Id != userId)
        {
            _logger.LogUnauthorizedAccess("user", userId, userId);
            throw ExceptionHelper.CreateUnauthorizedException("user", userId);
        }

        var isDeleted = await _userRepository.DeleteAsync(userId);
        if (isDeleted == null)
        {
            _logger.LogOperationFailure("delete", "user", userId);
            throw ExceptionHelper.CreateOperationException("user", userId, "delete");
        }

        _logger.LogOperationSuccess("deleted", "user", userId);
        return _userMapper.MapToDTO(userToDelete);
    }
}