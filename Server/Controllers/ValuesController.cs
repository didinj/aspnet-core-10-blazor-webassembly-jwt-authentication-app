using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ValuesController : ControllerBase
{
    [HttpGet("public")]
    public IActionResult PublicValue()
        => Ok("This is a public value");

    [Authorize]
    [HttpGet("secure")]
    public IActionResult SecureValue()
        => Ok("This is a secure value only for authenticated users");

    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public IActionResult AdminValue()
        => Ok("This is ADMIN-only data");
}
