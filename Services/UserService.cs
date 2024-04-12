using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Services.Interfaces;

namespace PlanIT.API.Services;

public class UserService : IUserService
{
    private readonly IMapper<User, UserDTO> _userMapper;
    private readonly IMapper<User, UserRegDTO> _userRegMapper;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IMapper<User, UserDTO> userMapper
        , IMapper<User, UserRegDTO> UserRegMapper,
        IUserRepository userRepository,
        ILogger<UserService> logger)
    {
        _userMapper = userMapper;
        _userRegMapper = UserRegMapper;
        _userRepository = userRepository;
        _logger = logger;
    }


    // Registrerer en ny bruker
    public async Task<UserDTO?> RegisterUserAsync(UserRegDTO userRegDTO)
    {
        _logger.LogDebug("Starting registration process for new user.");

        // Mapper UserRegDTO til User-modellen
        var newUser = _userRegMapper.MapToModel(userRegDTO);
        _logger.LogDebug("User data mapped from DTO to User model.");

        // Genererer salt og hash-verdi for passordet
        newUser.Salt = BCrypt.Net.BCrypt.GenerateSalt();
        newUser.HashedPassword = BCrypt.Net.BCrypt.HashPassword(userRegDTO.Password, newUser.Salt);
        _logger.LogDebug("Password hashing completed.");

        // Legger til den nye brukeren i databasen og henter resultatet
        var addedUser = await _userRepository.AddAsync(newUser);
        if (addedUser == null)
        {
            _logger.LogError("Failed to add new user to the database.");
            return null;
        }

        _logger.LogInformation("New user registered successfully with ID {UserId}.", addedUser.Id);

        // Mapper den nye brukeren til UserDto og returnerer den
        var userDTO = _userMapper.MapToDTO(addedUser);
        _logger.LogDebug("New user data mapped to UserDTO.");

        return userDTO;
    }


    // Henter alle brukere, med paginering
    public async Task<ICollection<UserDTO>> GetAllAsync(int pageNr, int pageSize)
    {
        _logger.LogDebug("Fetching all users with pagination: Page {PageNumber}, Size {PageSize}", pageNr, pageSize);

        var usersFromRepository = await _userRepository.GetAllAsync(pageNr, pageSize);

        if (usersFromRepository == null || usersFromRepository.Count == 0)
        {
            _logger.LogWarning("No users found for page {PageNumber} with page size {PageSize}.", pageNr, pageSize);
            return new List<UserDTO>(); // Returner en tom liste hvis ingen brukere finnes
        }

        _logger.LogInformation("Successfully retrieved {Count} users for page {PageNumber}.", usersFromRepository.Count, pageNr);

        // Mapper brukerdataene til DTO-format
        var userDTOs = usersFromRepository.Select(user => _userMapper.MapToDTO(user)).ToList();
        _logger.LogDebug("Completed mapping of users to DTOs.");

        return userDTOs;
    }


    // Henter bruker basert på ID
    public async Task<UserDTO?> GetByIdAsync(int userId)
    {
        _logger.LogDebug("Attempting to retrieve user with ID: {UserId}", userId);

        var userFromRepository = await _userRepository.GetByIdAsync(userId);
        if (userFromRepository == null)
        {
            _logger.LogWarning("User with ID {UserId} not found.", userId);
            return null;
        }

        _logger.LogInformation("User with ID {UserId} retrieved successfully.", userId);
        return _userMapper.MapToDTO(userFromRepository);
    }


    // Oppdaterer bruker
    public async Task<UserDTO?> UpdateAsync(int userId, UserDTO userDTO)
    {
        // Sjekker om brukeren eksisterer
        var existingUser = await _userRepository.GetByIdAsync(userId);

        if (existingUser == null)
        {
            _logger.LogWarning("Failed to update user: User not found with ID: {UserId}", userId);
            return null;
        }

        var userToUpdate = _userMapper.MapToModel(userDTO);

        // Oppdaterer brukeren
        var updatedUser = await _userRepository.UpdateAsync(userId, userToUpdate);
        if (updatedUser == null)
        {
            _logger.LogError("Failed to update user with ID: {UserId}", userId);
            return null;
        }

        _logger.LogInformation("Successfully updated user with ID: {UserId}", userId);

        return _userMapper.MapToDTO(updatedUser);
    }


    // Sletter en bruker
    public async Task<UserDTO?> DeleteAsync(int userId)
    {
        // Sjekker om brukeren eksisterer
        var userToDelete = await _userRepository.GetByIdAsync(userId);
        if (userToDelete == null)
        {
            _logger.LogWarning("Failed to delete user: User not found with ID: {UserId}", userId);
            return null;
        }

        // Sletter brukeren fra databasen
        var isDeleted = await _userRepository.DeleteAsync(userId);
        if (isDeleted == null)
        {
            _logger.LogError("Error deleting user with ID: {UserId}", userId);
            return null;
        }

        _logger.LogInformation("Successfully deleted user with ID: {UserId}", userId);

        return _userMapper.MapToDTO(userToDelete);
    }
}