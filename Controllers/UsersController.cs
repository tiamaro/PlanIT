using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanIT.API.Extensions;
using PlanIT.API.Middleware;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Services.Interfaces;

namespace PlanIT.API.Controllers;

// UsersController - API Controller for user management:
// - The controller handles all requests related to user data, including registration,
//   updating, deletion, and retrieval of user information. It receives an instance of IUserService
//   as part of the constructor to perform operations related to users.
//
// Policy:
// - "Bearer": Requires that all calls to this controller are authenticated with a valid JWT token
//   that meets the requirements defined in the "Bearer" authentication policy. This ensures that only
//   authenticated users can access the endpoints defined in this controller.
//
// HandleExceptionFilter:
// - This filter is attached to the controller to catch and handle exceptions in a centralized manner.
//
// Requests starting with "api/v1/Users" will be routed to methods defined in this controller.

[Authorize(Policy = "Bearer")]
[Route("api/v1/[controller]")]
[ApiController]
[ServiceFilter(typeof(HandleExceptionFilter))]  // Uses HandleExceptionFilter to handle exceptions
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }


    // Registers a new user
    // POST /api/v1/Users/register
    [AllowAnonymous]
    [HttpPost("register", Name = "AddUser")]
    public async Task<ActionResult<UserDTO>> AddUserAsync(UserRegDTO userRegDTO)
    {
        // Checks if the model state is valid after model binding and validation
        if (!ModelState.IsValid)
        {
            _logger.LogError("Invalid model state in AddUserAsync");
            return BadRequest(ModelState);
        }

        // registrating user
        var userDTO = await _userService.RegisterUserAsync(userRegDTO);

        // check if registration was successful 
        return userDTO != null
            ? Ok(userDTO)
            : BadRequest("Failed to register new user");
    }


    // At the moment not in use. ONLY FOR TESTING
    // Retrieves a paginated list of users.
    // GET: /api/v1/Users?pageNr=1&pageSize=10

    [HttpGet(Name = "GetUsers")]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsersAsync(int pageNr, int pageSize)
    {
        var users = await _userService.GetAllAsync(pageNr, pageSize);
        return Ok(users);
    }


    // At the moment not in use. ONLY FOR TESTING
    // Retrieves a specific user by its ID.
    // GET /api/v1/Users/{id}

    [HttpGet("{userId}", Name = "GetUsersById")]
    public async Task<ActionResult<UserDTO>> GetUsersByIdASync(int userId)
    {
        var user = await _userService.GetByIdAsync(userId);

        return user != null
            ? Ok(user)
            : NotFound("User not found");
    }


    // Retrieves information about the logged-in user by using a user ID.
    // extracted from the JWT token, which is then stored in HttpContext.Items by middleware.
    [HttpGet("profile", Name = "GetUserProfile")]
    public async Task<ActionResult<UserDTO>> GetUserProfileAsync()
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        var user = await _userService.GetByIdAsync(userId);

        return user != null
            ? Ok(user)
            : NotFound("User not found");
    }


    // Updates a user based on the provided ID.
    // PUT /api/v1/Users/{id}
    [HttpPut(Name = "UpdateUser")]
    public async Task<ActionResult<UserDTO>> UpdateUserAsync(UserDTO updatedUserDTO)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

      
        var updatedUserResult = await _userService.UpdateAsync(userId, updatedUserDTO);

        // Returns updated user, or an error message if update fails
        return updatedUserResult != null
            ? Ok(updatedUserResult)
            : NotFound("Unable to update the user");
    }


    // Deletes a user based on the provided ID.
    // DELETE /api/v1/Users/{id}
    [HttpDelete(Name = "DeleteUser")]
    public async Task<IActionResult> DeleteUserAsync()
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        var deletedUserResult = await _userService.DeleteAsync(userId);

        // Returns deleted user, or an error message if deletion fails
        return deletedUserResult != null
            ? Ok(deletedUserResult)
            : NotFound("Unable to delete user");
    }
}