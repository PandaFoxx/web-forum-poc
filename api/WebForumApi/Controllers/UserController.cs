using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebForumApi.Application.User.Login;

namespace WebForumApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UserController(IMediator mediator)
  : ControllerBase

{
  [HttpPost]
  public async Task<IActionResult> Login(
    UserLoginRequest request,
    CancellationToken cancellationToken
  )
  {
    return Ok(await mediator.Send(request, cancellationToken));
  }
}
