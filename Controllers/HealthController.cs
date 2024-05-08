using Microsoft.AspNetCore.Mvc;
namespace PlanIT.API.Controllers;

// Provides an endpoint to check the health of the API.
[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult GetHealthStatus()
    {
        return Ok("API is up and running!");
    }
}