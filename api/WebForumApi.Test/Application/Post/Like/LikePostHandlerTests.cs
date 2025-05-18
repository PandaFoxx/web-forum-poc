using System.Security;
using FluentValidation;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using WebForumApi.Application.Post.Like;
using WebForumApi.Domain;

namespace WebForumApi.Test;

public sealed class LikePostHandlerTests
{
  private readonly ISessionManager sessionManager;
  private readonly IDataAccess dataAccess;
  private readonly IValidator<LikePostRequest> validator;
  private readonly LikePostHandler handler;

  public LikePostHandlerTests()
  {
    sessionManager = Substitute.For<ISessionManager>();
    dataAccess = Substitute.For<IDataAccess>();
    validator = Substitute.For<IValidator<LikePostRequest>>();

    handler = new LikePostHandler(sessionManager, dataAccess, validator);
  }

  [Fact]
  public async Task Handle_Success()
  {
    var request = new LikePostRequest
    {
      AccessToken = "token",
      PostGuid = Guid.NewGuid(),
      Unlike = false
    };

    var userGuid = Guid.NewGuid();

    var dto = new List<LikePostDto>
    {
      new ()
      {
        post_like_id = 100
      }
    };

    sessionManager
      .GetCachedUser(request.AccessToken)
      .Returns(userGuid);

    dataAccess
      .ExecuteProcedureAsync<LikePostDto>(Arg.Any<string>(), Arg.Any<Dictionary<string, object>>())
      .Returns(dto);

    var sut = await handler.Handle(request, CancellationToken.None);

    Assert.Equal(dto.First().post_like_id, sut.PostLikeId);
  }

  [Fact]
  public async Task Handle_Forbidden()
  {
    var request = new LikePostRequest
    {
      AccessToken = "token",
      PostGuid = Guid.NewGuid(),
      Unlike = false
    };

    var userGuid = Guid.NewGuid();

    sessionManager
      .GetCachedUser(request.AccessToken)
      .Returns(userGuid);

    dataAccess
      .ExecuteProcedureAsync<LikePostDto>(
        Arg.Any<string>(),
        Arg.Any<Dictionary<string, object>>())
      .Throws(new Exception("error"));

    await Assert.ThrowsAsync<SecurityException>(async () =>
      await handler.Handle(request, CancellationToken.None));
  }
}
