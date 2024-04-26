using System.Security.Claims;

namespace PlanIT.API.Services.Interfaces;

public interface IEmailAuth
{
    string GenerateJwtToken(int inviteId, int eventId);

    ClaimsPrincipal DecodeToken(string token);

    (int inviteId, int eventId) ValidateAndExtractClaims(string token);

}