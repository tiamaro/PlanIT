using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanIT.API.Extensions;
using PlanIT.API.Middleware;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Services.Interfaces;

namespace PlanIT.API.Controllers;



// InvitesController - API Controller for invitasjonshåndtering:
// - Kontrolleren håndterer alle forespørsler relatert til invitasjoner, inkludert registrering,
//   oppdatering, sletting og henting av invitasjonsinformasjon. Den tar imot en instans av IService
//   som en del av konstruktøren for å utføre operasjoner relatert til invitasjoner.
//
// Policy:
// - "Bearer": Krever at alle kall til denne kontrolleren er autentisert med et gyldig JWT-token
//   som oppfyller kravene definert i "Bearer" autentiseringspolicy. Dette sikrer at bare
//   autentiserte brukere kan aksessere endepunktene definert i denne kontrolleren.
//
// HandleExceptionFilter:
// - Dette filteret er tilknyttet kontrolleren for å fange og behandle unntak på en sentralisert måte.
//
// Forespørsler som starter med "api/v1/Invites" vil bli rutet til metoder definert i denne kontrolleren.

[Authorize(Policy = "Bearer")]
[Route("api/v1/[controller]")]
[ApiController]
[ServiceFilter(typeof(HandleExceptionFilter))]  // Bruker HandleExceptionFilter for å håndtere unntak

public class InvitesController : ControllerBase
{
    private readonly IService<InviteDTO> _inviteService;
    private readonly ILogger<InvitesController> _logger;


    public InvitesController(IService<InviteDTO> inviteService, ILogger<InvitesController> logger)
    {
        _inviteService = inviteService;
        _logger = logger;

    }

    // Endepunkt for registrering av ny invitasjon
    // POST /api/v1/Invites/register
    [HttpPost("register", Name = "AddInvites")]
    public async Task<ActionResult<InviteDTO>> AddInviteAsync(InviteDTO newInviteDTO)
    {
        // Henter brukerens ID fra HttpContext.Items som ble lagt til av middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        // Sjekk om modelltilstanden er gyldig etter modellbinding og validering
        if (!ModelState.IsValid)
        {
            _logger.LogError("Invalid model state in AddInviteAsync");
            return BadRequest(ModelState);
        }

        // Registrer invitasjonen
        var addedInvite = await _inviteService.CreateAsync(userId, newInviteDTO);

        // Sjekk om invitasjonsregistreringen var vellykket
        return addedInvite != null
            ? Ok(addedInvite)
            : BadRequest("Failed to register new invite");
    }


    // Henter en liste over invitasjoner
    // GET: /api/v1/Invites?pageNr=1&pageSize=10
    [HttpGet(Name = "GetInvites")]
    public async Task<ActionResult<IEnumerable<InviteDTO>>> GetInvitesAsync(int pageNr, int pageSize)
    {
        // Henter brukerens ID fra HttpContext.Items som ble lagt til av middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        var invites = await _inviteService.GetAllAsync(userId, pageNr, pageSize);

        return invites != null
            ? Ok(invites)
            : NotFound("No registered invites found.");
    }


    // Henter et arrangement basert på invitasjonens ID
    // GET /api/v1/Invites/1
    [HttpGet("{inviteId}", Name = "GetInvitesById")]
    public async Task<ActionResult<InviteDTO>> GetInvitesByIdASync(int inviteId)
    {
        // Henter brukerens ID fra HttpContext.Items som ble lagt til av middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        // Hent invitasjonen fra tjenesten
        var existingInvite = await _inviteService.GetByIdAsync(userId, inviteId);

        return existingInvite != null
            ? Ok(existingInvite)
            : NotFound("Invite not found");
    }


    // Oppdaterer en invitasjon basert på inviteID
    // PUT /api/v1/Invites/4
    [HttpPut("{inviteId}", Name = "UpdateInvite")]
    public async Task<ActionResult<InviteDTO>> UpdateInviteAsync(int inviteId, InviteDTO updatedInviteDTO)
    {
        // Henter brukerens ID fra HttpContext.Items som ble lagt til av middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        // Prøver å oppdatere invitasjonen med den nye informasjonen
        var updatedInviteResult = await _inviteService.UpdateAsync(userId, inviteId, updatedInviteDTO);

        // Returnerer oppdatert invitasjonsdata, eller en feilmelding hvis oppdateringen mislykkes
        return updatedInviteResult != null
            ? Ok(updatedInviteResult)
            : NotFound("Unable to update the invite or the invite does not belong to the user");
    }


    // Sletter en invitasjon basert på invitasjonens ID
    // DELETE /api/v1/Invites/2
    [HttpDelete("{inviteId}", Name = "DeleteInvite")]
    public async Task<ActionResult<InviteDTO>> DeleteInviteAsync(int inviteId)
    {
        // Henter brukerens ID fra HttpContext.Items som ble lagt til av middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        // Prøver å slette invitasjonen.
        var deletedInviteResult = await _inviteService.DeleteAsync(userId, inviteId);

        return deletedInviteResult != null
            ? Ok(deletedInviteResult)
            : BadRequest("Unable to delete invite or the invite does not belong to the user");
    }
}