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

        // Mapper UserRegDTO til User-modellen
        var newUser = _userRegMapper.MapToModel(userRegDTO);

        // Genererer salt og hash-verdi for passordet
        newUser.Salt = BCrypt.Net.BCrypt.GenerateSalt();
        newUser.HashedPassword = BCrypt.Net.BCrypt.HashPassword(userRegDTO.Password, newUser.Salt);

        // Legger til den nye brukerern i databasen og henter resultatet
        var addedUser = await _userRepository.AddAsync(newUser);

        // Mapper den nye brukeren til UserDto og returnerer den
        return _userMapper.MapToDTO(addedUser!);

    }


    // Henter alle brukere, med paginering
    public async Task<ICollection<UserDTO>> GetAllAsync(int pageNr, int pageSize)
    {
        // Henter brukerinformasjon fra repository med paginering
        var usersFromRepository = await _userRepository.GetAllAsync(1, 10);

        // Mapper brukerdataene til userDto-format
        var userDTOs = usersFromRepository.Select(user => _userMapper.MapToDTO(user)).ToList();
        return userDTOs;
    }


    // Henter bruker basert på ID
    public async Task<UserDTO?> GetByIdAsync(int userId)
    {
        var userFromRepository = await _userRepository.GetByIdAsync(userId);
        return userFromRepository != null ? _userMapper.MapToDTO(userFromRepository) : null;
    }


    // Oppdaterer bruker
    public async Task<UserDTO?> UpdateAsync(int userId, UserDTO userDTO)
    {
        var existingUser = await _userRepository.GetByIdAsync(userId);

        if (existingUser == null) return null;

        var userToUpdate = _userMapper.MapToModel(userDTO);

        // Sørger for at IDen som brukes er den som er autorisert fra JWT-token
        userToUpdate.Id = userId;

        var updatedUser = await _userRepository.UpdateAsync(userId, userToUpdate);

        return updatedUser != null ? _userMapper.MapToDTO(updatedUser) : null;
    }



    // Sletter en bruker
    public async Task<UserDTO?> DeleteAsync(int userId)
    {
        var userToDelete = await _userRepository.GetByIdAsync(userId);

        // Sjekker om brukeren eksisterer
        if (userToDelete == null) return null;

        // Sletter brukeren fra databasen og mapper den til UserDTO for retur                 
        var isDeleted = await _userRepository.DeleteAsync(userId);
        return isDeleted != null ? _userMapper.MapToDTO(userToDelete) : null;

    }

}