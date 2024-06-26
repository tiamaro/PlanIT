﻿using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Services.Interfaces;
using PlanIT.API.Utilities; 

namespace PlanIT.API.Services;

// Service class for handling user information.
// Exceptions are caught by a middleware: HandleExceptionFilter
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

   
    public async Task<ICollection<UserDTO>> GetAllAsync(int pageNr, int pageSize)
    {
        _logger.LogDebug($"Retrieving all users.");

        var usersFromRepository = await _userRepository.GetAllAsync(pageNr, pageSize);
        if (!usersFromRepository.Any())
        {
            _logger.LogWarning("No users found during pagination retrieval.");
            return new List<UserDTO>();
        }

        var userDTOs = usersFromRepository.Select(user => _userMapper.MapToDTO(user)).ToList();
        return userDTOs;
    }

   
    public async Task<UserDTO?> GetByIdAsync(int userId)
    {
        _logger.LogDebug($"Retrieving user with ID {userId}");

        var userFromRepository = await _userRepository.GetByIdAsync(userId);
        if (userFromRepository == null)
        {
            _logger.LogNotFound("user", userId);
            throw ExceptionHelper.CreateNotFoundException("user", userId);
        }

        return _userMapper.MapToDTO(userFromRepository);
    }


    public async Task<UserDTO?> UpdateAsync(int userId, UserDTO userDTO)
    {
        _logger.LogDebug($"Updating user with ID {userId}");

        var existingUser = await _userRepository.GetByIdAsync(userId);
        if (existingUser == null)
        {
            _logger.LogNotFound("user", userId);
            throw ExceptionHelper.CreateNotFoundException("user", userId);
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


   
    public async Task<UserDTO?> DeleteAsync(int userId)
    {
        _logger.LogDebug($"Deleting user with ID {userId}");

        var userToDelete = await _userRepository.DeleteAsync(userId);
        if (userToDelete == null)
        {
            _logger.LogOperationFailure("delete", "user", userId);
            throw ExceptionHelper.CreateOperationException("user", userId, "delete");
        }

        _logger.LogOperationSuccess("deleted", "user", userId);
        return _userMapper.MapToDTO(userToDelete);
    }
}