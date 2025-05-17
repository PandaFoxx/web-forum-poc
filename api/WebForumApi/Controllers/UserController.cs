using Microsoft.AspNetCore.Mvc;

namespace WebForumApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UserController : ControllerBase
{
  [HttpPost]
  public async Task<IActionResult> Login()
  {
    return Ok();
  }
}
