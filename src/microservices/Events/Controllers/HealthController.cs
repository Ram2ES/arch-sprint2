using Microsoft.AspNetCore.Mvc;

namespace Events.Controllers;

[ApiController]
[Route("api/events/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { status = true });
    }
}
