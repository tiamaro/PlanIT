using System.Security.Claims;

namespace PlanIT.API.Services.Interfaces;

public interface IJWTEmailAuth
{
    string GenerateJwtToken(int inviteId, int eventId);
    ClaimsPrincipal DecodeToken(string token);

}
