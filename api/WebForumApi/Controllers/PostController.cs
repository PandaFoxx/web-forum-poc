using System.Diagnostics.CodeAnalysis;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebForumApi.Application.Post.Add;
using WebForumApi.Application.Post.Comment;

namespace WebForumApi.Controllers;

[ExcludeFromCodeCoverage]
[ApiController]
[Route("api/[controller]")]
public sealed class PostController(
  IMediator mediator
)
  : ControllerBase
{
  [HttpPost]
  public async Task<IActionResult> AddPost(
    [FromHeader(Name = "X-Access-Token")] string accessToken,
    AddPostRequest request,
    CancellationToken cancellationToken
  )
  {
    request.AccessToken = accessToken;

    return Ok(await mediator.Send(request, cancellationToken));
  }

  [HttpPost("comment")]
  public async Task<IActionResult> AddPostComment(
    [FromHeader(Name = "X-Access-Token")] string accessToken,
    AddPostCommentRequest request,
    CancellationToken cancellationToken
  )
  {
    request.AccessToken = accessToken;

    return Ok(await mediator.Send(request, cancellationToken));
  }
}
