using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Healthchecks;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("Healthy");
}