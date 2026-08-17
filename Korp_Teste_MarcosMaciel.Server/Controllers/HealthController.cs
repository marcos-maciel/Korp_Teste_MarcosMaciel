using Microsoft.AspNetCore.Mvc;

namespace Korp_Teste_MarcosMaciel.Server.Controllers;

[ApiController]
[Route("api")]
public class HealthController : ControllerBase
{
    [HttpGet("health")]
    public IActionResult GetHealth()
    {
        return Ok(new
        {
            status = "ok",
            module = "backend-base",
            timestamp = DateTime.UtcNow
        });
    }
}
