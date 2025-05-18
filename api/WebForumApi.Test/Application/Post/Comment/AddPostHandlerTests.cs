using System.Security;
using FluentValidation;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using WebForumApi.Application.Post.Comment;
using WebForumApi.Domain;

namespace WebForumApi.Test;

public sealed class AddPostCommentHandlerTests
{
  private readonly ISessionManager sessionManager;
  private readonly IDataAccess dataAccess;
  private readonly IValidator<AddPostCommentRequest> validator;
  private readonly AddPostCommentHandler handler;

  public AddPostCommentHandlerTests()
  {
    sessionManager = Substitute.For<ISessionManager>();
    dataAccess = Substitute.For<IDataAccess>();
    validator = Substitute.For<IValidator<AddPostCommentRequest>>();

    handler = new AddPostCommentHandler(sessionManager, dataAccess, validator);
  }

  [Fact]
  public async Task Handle_Success()
  {
    var request = new AddPostCommentRequest
    {
      AccessToken = "token",
      PostGuid = Guid.NewGuid(),
      Content = "content"
    };

    var userGuid = Guid.NewGuid();

    var dto = new List<AddPostCommentDto>
    {
      new ()
      {
        post_comment_guid = Guid.NewGuid()
      }
    };

    sessionManager
      .GetCachedUser(request.AccessToken)
      .Returns(userGuid);

    dataAccess
      .ExecuteProcedureAsync<AddPostCommentDto>(Arg.Any<string>(), Arg.Any<Dictionary<string, object>>())
      .Returns(dto);

    var sut = await handler.Handle(request, CancellationToken.None);

    Assert.Equal(dto.First().post_comment_guid, sut.PostCommentGuid);
  }

  [Fact]
  public async Task Handle_Forbidden()
  {
    var request = new AddPostCommentRequest
    {
      AccessToken = "token",
      PostGuid = Guid.NewGuid(),
      Content = "content"
    };

    var userGuid = Guid.NewGuid();

    sessionManager
      .GetCachedUser(request.AccessToken)
      .Returns(userGuid);

    dataAccess
      .ExecuteProcedureAsync<AddPostCommentDto>(
        Arg.Any<string>(),
        Arg.Any<Dictionary<string, object>>())
      .Throws(new Exception("error"));

    await Assert.ThrowsAsync<SecurityException>(async () =>
      await handler.Handle(request, CancellationToken.None));
  }
}
