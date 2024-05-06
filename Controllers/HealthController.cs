using Microsoft.AspNetCore.Mvc;

namespace PlanIT.API.Controllers;


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