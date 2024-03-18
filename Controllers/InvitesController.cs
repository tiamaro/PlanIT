using Microsoft.AspNetCore.Mvc;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Services;
using PlanIT.API.Services.Interfaces;

namespace PlanIT.API.Controllers;


// Kontrolleren for API-versjon 1 som definerer adressen (URL) til API-et.
// [ApiController] indikerer at klassen fungerer som en kontroller i systemet og arver fra ControllerBase.
// Tar imot en InviteService-innstans som en del av konstruktøren for å utføre invitasjonsrelaterte operasjoner.
// api/v1/Invites

[Route("api/v1/[controller]")]
[ApiController]

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
        try
        {
            // Sjekk om modelltilstanden er gyldig etter modellbinding og validering
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state in AddInviteAsync");

                // Hvis modelltilstanden er ugyldig, returner en BadRequest sammen med ModelState-feilene
                return BadRequest(ModelState);
            }

            // Registrer invitasjonen ved å bruke de oppgitte detaljene for invitasjonsregistrering
            var addedInvite = await _inviteService.CreateAsync(newInviteDTO);

            // Sjekk om invitasjonsregistreringen var vellykket
            return addedInvite != null
                ? Ok(addedInvite)
                : BadRequest("Failed to register new invite");
        }
        catch (Exception ex) // Generell Exception-håndtering for uventede feil
        {
            _logger.LogError("An unknown error occured: " + ex.Message);
            return StatusCode(500, "An unknown error occured, please try again later");
        }
    }


    // Henter en liste over invitasjoner i form av InviteDTO-objekter med paginering.
    // Parametrene 'pageNr' og 'pageSize' angir henholdsvis sidenummer og størrelse på siden.
    // Returnerer en ActionResult med en IEnumerable av InviteDTO-objekter.
    // GET: /api/v1/Invites?pageNr=1&pageSize=10
    [HttpGet(Name = "GetInvites")]
    public async Task<ActionResult<IEnumerable<InviteDTO>>> GetInvitesAsync(int pageNr, int pageSize)
    {
        var invites = await _inviteService.GetAllAsync(pageNr, pageSize);

        return invites != null
            ? Ok(invites)
            : NotFound("No registered invites found.");
    }


    // Henter et arrangement basert på invitasjonens ID ved hjelp av InviteService.
    // Returnerer en ActionResult med en InviteDTO hvis invitasjonen ble funnet,
    // ellers returneres NotFound hvis invitasjonen ikke ble funnet.
    // GET /api/v1/Invites/1
    [HttpGet("{inviteId}", Name = "GetInvitesById")]
    public async Task<ActionResult<InviteDTO>> GetInvitesByIdASync(int inviteId)
    {
        // Hent invitasjonen fra tjenesten
        var existingInvite = await _inviteService.GetByIdAsync(inviteId);

        return existingInvite != null
            ? Ok(existingInvite)
            : NotFound("Invite not found");
    }


    // Oppdaterer en invitasjon basert på invitasjonens ID ved å bruke InviteService.
    // Returnerer en ActionResult med en oppdatert InviteDTO hvis oppdateringen var vellykket, 
    // ellers returneres NotFound hvis invitasjonen ikke ble funnet.
    // PUT /api/v1/Invites/4
    [HttpPut("{inviteId}", Name = "UpdateInvite")]
    public async Task<ActionResult<InviteDTO>> UpdateInviteAsync(int inviteId, InviteDTO updatedInviteDTO)
    {
        // Hent invitasjon fra tjenesten
        var existingInvite = await _inviteService.GetByIdAsync(inviteId);

        // Sjekk om invitasjonen eksisterer
        if (existingInvite == null) return NotFound("Invite not found");

        try
        {
            // Hvis invitasjonen er riktig, fortsett med oppdateringen.
            var updatedInviteResult = await _inviteService.UpdateAsync(inviteId, updatedInviteDTO);
            return updatedInviteResult != null
                ? Ok(updatedInviteResult)
                : NotFound("Unable to update the invite");
        }
        catch (Exception ex) // Generell Exception-håndtering for uventede feil
        {
            _logger.LogError("An unknown error occured: " + ex.Message);
            return StatusCode(500, "An unknown error occured, please try again later");
        }

    }


    // Sletter en invitasjon basert på invitasjonens ID ved å bruke InviteService.
    // Returnerer en ActionResult med en slettet InviteDTO hvis slettingen var vellykket, 
    // ellers returneres BadRequest hvis invitasjonen ikke kunne slettes.
    // DELETE /api/v1/Invites/2
    [HttpDelete("{inviteId}", Name = "DeleteInvite")]
    public async Task<ActionResult<InviteDTO>> DeleteInviteAsync(int inviteId)
    {
        // Hent invitasjon fra tjenesten
        var existingInvite = await _inviteService.GetByIdAsync(inviteId);

        // Sjekk om invitasjonen eksisterer
        if (existingInvite == null) return NotFound("Invite not found");

        try
        {
            // Hvis invitasjonen er riktig, fortsett med slettingen.
            var deletedInviteResult = await _inviteService.DeleteAsync(inviteId);
            return deletedInviteResult != null
                ? Ok(deletedInviteResult)
                : BadRequest("Unable to delete invite");
        }
        catch (Exception ex) // Generell Exception-håndtering for uventede feil
        {
            _logger.LogError("An unknown error occured: " + ex.Message);
            return StatusCode(500, "An unknown error occured, please try again later");
        }

    }
}