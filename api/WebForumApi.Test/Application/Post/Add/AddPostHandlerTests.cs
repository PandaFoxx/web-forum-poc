using System.Security;
using FluentValidation;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using WebForumApi.Application.Post.Add;
using WebForumApi.Domain;

namespace WebForumApi.Test;

public sealed class AddPostHandlerTests
{
  private readonly ISessionManager sessionManager;
  private readonly IDataAccess dataAccess;
  private readonly IValidator<AddPostRequest> validator;
  private readonly AddPostHandler handler;

  public AddPostHandlerTests()
  {
    sessionManager = Substitute.For<ISessionManager>();
    dataAccess = Substitute.For<IDataAccess>();
    validator = Substitute.For<IValidator<AddPostRequest>>();

    handler = new AddPostHandler(sessionManager, dataAccess, validator);
  }

  [Fact]
  public async Task Handle_Success()
  {
    var request = new AddPostRequest
    {
      AccessToken = "token",
      Title = "title",
      Content = "content"
    };

    var userGuid = Guid.NewGuid();

    var dto = new List<AddPostDto>
    {
      new ()
      {
        post_guid = Guid.NewGuid()
      }
    };

    sessionManager
      .GetCachedUser(request.AccessToken)
      .Returns(userGuid);

    dataAccess
      .ExecuteProcedureAsync<AddPostDto>(Arg.Any<string>(), Arg.Any<Dictionary<string, object>>())
      .Returns(dto);

    var sut = await handler.Handle(request, CancellationToken.None);

    Assert.Equal(dto.First().post_guid, sut.PostGuid);
  }

  [Fact]
  public async Task Handle_Forbidden()
  {
    var request = new AddPostRequest
    {
      AccessToken = "token",
      Title = "title",
      Content = "content"
    };

    var userGuid = Guid.NewGuid();

    sessionManager
      .GetCachedUser(request.AccessToken)
      .Returns(userGuid);

    dataAccess
      .ExecuteProcedureAsync<AddPostDto>(
        Arg.Any<string>(),
        Arg.Any<Dictionary<string, object>>())
      .Throws(new Exception("error"));

    await Assert.ThrowsAsync<SecurityException>(async () =>
      await handler.Handle(request, CancellationToken.None));
  }
}
