using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanIT.API.Extensions;
using PlanIT.API.Middleware;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Services.Interfaces;

namespace PlanIT.API.Controllers;


// ContactsController - API Controller for managing contacts:
// - The controller handles all requests related to contacts, including registration,
//   updating, deletion, and retrieval of date information. It receives an instance of IService
//   as part of the constructor to perform operations related to contacts.
//
// Policy:
// - "Bearer": Requires that all calls to this controller are authenticated with a valid JWT token
//   that meets the requirements defined in the "Bearer" authentication policy. This ensures that only
//   authenticated users can access the endpoints defined in this controller.
//
// HandleExceptionFilter:
// - This filter is attached to the controller to catch and handle exceptions in a centralized manner.
//
// Requests starting with "api/v1/Contacts" will be routed to methods defined in this controller.



[Authorize(Policy = "Bearer")]
[Route("api/v1/[controller]")]
[ApiController]
[ServiceFilter(typeof(HandleExceptionFilter))]  // Uses HandleExceptionFilter to handle exceptions
public class ContactsController : ControllerBase
{
    private readonly IService<ContactDTO> _contactService;
    private readonly ILogger<ContactsController> _logger;

    public ContactsController(IService<ContactDTO> contactService,
        ILogger<ContactsController> logger)
    {
        _contactService = contactService;
        _logger = logger;
    }




    // Registers a new contact
    // POST /api/v1/Contacts/register
    [HttpPost("register", Name = "addContact")]
    public async Task<ActionResult<ContactDTO>> AddContactAsync(ContactDTO newContactDTO)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        // Checks if the model state is valid after model binding and validation
        if (!ModelState.IsValid)
        {
            _logger.LogError("Invalid model state in AddContactAsync");
            return BadRequest(ModelState);
        }

        
        var addedContact = await _contactService.CreateAsync(userId,newContactDTO);


        // Returns new contact, or an error message if registration fails
        return addedContact != null
            ? Ok(addedContact) 
            : BadRequest("Failed to register new contact");
    }


    // Retrieves a paginated list of contacts
    // GET: /api/v1/Contacts?pageNr=1&pageSize=10
    [HttpGet(Name = "GetContacts")]
    public async Task <ActionResult<IEnumerable<ContactDTO>>> GetContactsAsync(int pageNr, int pageSize)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        var allContacts = await _contactService.GetAllAsync(userId,pageNr, pageSize);

        // Returns list of contacts, or an error message if not found

        return Ok(allContacts ?? new List<ContactDTO>()); // Return an empty list instead of null

    }

    // Retrieves a specific contact by its ID.
    // GET /api/v1/Contacts/{id}
    [HttpGet("{contactId}", Name = "GetContactById")]
    public async Task<ActionResult<ContactDTO>> GetContactByIdAsync(int contactId)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);


        
        var exsistingContact = await _contactService.GetByIdAsync(userId, contactId);

        // Returns contact, or an error message if not found
        return exsistingContact != null
            ? Ok(exsistingContact)
            : NotFound("contact not found.");

    }


    // Updates a contact based on the provided ID.
    // PUT /api/v1/Contacts/{id}
    [HttpPut("{contactId}", Name = "UpdateContact")]
    public async Task<ActionResult<ContactDTO>> UpdateContactAsync(int contactId, ContactDTO updatedEventDTO)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);


        
        var updatedContact =await _contactService.UpdateAsync(userId, contactId, updatedEventDTO);


        // Returns updated contact, or an error message if update fails
        return updatedContact != null
           ? Ok(updatedContact)
           : NotFound("Unable to update the contact or the contact does not belong to the user");


    }


    // Deletes a contact based on the provided ID.
    // DELETE /api/v1/Contacts/{id}
    [HttpDelete("{contactId}", Name = "DeleteContact")]
    public async Task<ActionResult<ContactDTO>> DeleteContactAsync(int contactId)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        
        var deletedContact = await _contactService.DeleteAsync(userId, contactId);

        // Returns deleted contact, or an error message if deletion fails
        return deletedContact != null
           ? Ok(deletedContact)
           : NotFound("Unable to delete contact or contact does not belong to the user");


    }
    








}
