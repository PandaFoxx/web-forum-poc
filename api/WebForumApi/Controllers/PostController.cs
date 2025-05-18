using System.Diagnostics.CodeAnalysis;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebForumApi.Application.Post.Add;
using WebForumApi.Application.Post.Comment;
using WebForumApi.Application.Post.Get;
using WebForumApi.Application.Post.Like;
using WebForumApi.Application.Post.Tag;

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

  [HttpPost("like")]
  public async Task<IActionResult> LikePost(
    [FromHeader(Name = "X-Access-Token")] string accessToken,
    LikePostRequest request,
    CancellationToken cancellationToken
  )
  {
    request.AccessToken = accessToken;

    await mediator.Send(request, cancellationToken);
    return NoContent();
  }

  [HttpPost("tag")]
  public async Task<IActionResult> TagPost(
    [FromHeader(Name = "X-Access-Token")] string accessToken,
    TagPostRequest request,
    CancellationToken cancellationToken
  )
  {
    request.AccessToken = accessToken;

    await mediator.Send(request, cancellationToken);
    return NoContent();
  }

  [HttpGet]
  public async Task<IActionResult> GetPosts(
    [FromQuery] GetPostsRequest request,
    CancellationToken cancellationToken
  )
  {
    return Ok(await mediator.Send(request, cancellationToken));
  }
}
