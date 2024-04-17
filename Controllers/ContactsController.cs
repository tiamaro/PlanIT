using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlanIT.API.Extensions;
using PlanIT.API.Middleware;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Services.Interfaces;

namespace PlanIT.API.Controllers;


[Authorize(Policy = "Bearer")]
[Route("api/v1/[controller]")]
[ApiController]
[ServiceFilter(typeof(HandleExceptionFilter))]  // Bruker HandleExceptionFilter for å håndtere unntak
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




    // Endepunkt for registrering av ny Contact
    // POST /api/v1/Contacts/register
    [HttpPost("register", Name = "addContact")]
    public async Task<ActionResult<ContactDTO>> AddContactAsync(ContactDTO newContactDTO)
    {
        // Sjekk om modelltilstanden er gyldig etter modellbinding og validering
        if (!ModelState.IsValid)
        {
            _logger.LogError("Invalid model state in AddContactAsync");
            return BadRequest(ModelState);
        }

        // Registrer Contact
        var addedContact = await _contactService.CreateAsync(newContactDTO);


        // Sjekk om Contactregistreringen var vellykket
        return addedContact != null
            ? Ok(addedContact) 
            : BadRequest("Failed to register new contact");
    }


    // Henter en liste over contacts
    // GET: /api/v1/Contacts?pageNr=1&pageSize=10
    [HttpGet(Name = "GetContacts")]
    public async Task <ActionResult<IEnumerable<ContactDTO>>> GetContactsAsync(int pageNr, int pageSize)
    {
        var allContacts = await _contactService.GetAllAsync(pageNr, pageSize);

        return allContacts != null
            ? Ok(allContacts)
            : BadRequest("No registered contacts found.");

    }

    // Henter contact basert på contactID
    // GET /api/v1/Contacts/1
    [HttpGet("{contactId}", Name = "GetContactById")]
    public async Task<ActionResult<ContactDTO>> GetContactByIdAsync(int contactId)
    {
        // Henter brukerens ID fra HttpContext.Items som ble lagt til av middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);


        // Hent contacts fra tjenesten, filtrert etter brukerens ID
        var exsistingContact = await _contactService.GetByIdAsync(userId, contactId);

        return exsistingContact != null
            ? Ok(exsistingContact)
            : BadRequest("contact not found.");

    }


    // Oppdaterer contact basert på contactID
    // PUT /api/v1/Contacts/4
    [HttpPut("{contactId}", Name = "UpdateContact")]
    public async Task<ActionResult<ContactDTO>> UpdateContactAsync(int contactId, ContactDTO updatedEventDTO)
    {
        // Henter brukerens ID fra HttpContext.Items som ble lagt til av middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);


        // Prøver å oppdatere contact med den nye informasjonen
        var updatedContact =await _contactService.UpdateAsync(userId, contactId, updatedEventDTO);


        // Returnerer oppdatert contact, eller en feilmelding hvis oppdateringen mislykkes
        return updatedContact != null
           ? Ok(updatedContact)
           : NotFound("Unable to update the contact or the contact does not belong to the user");


    }


    // Sletter et arrangement basert på contactID
    // DELETE /api/v1/Contacts/2
    [HttpDelete("{contactId}", Name = "DeleteContact")]
    public async Task<ActionResult<ContactDTO>> DeleteContactAsync(int contactId)
    {
        // Henter brukerens ID fra HttpContext.Items som ble lagt til av middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        // Prøver å slette contact
        var deletedContact = await _contactService.DeleteAsync(userId, contactId);

        return deletedContact != null
           ? Ok(deletedContact)
           : BadRequest("Unable to delete contact or contact does not belong to the user");


    }
    








}
