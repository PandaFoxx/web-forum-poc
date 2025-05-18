using System.Security;
using FluentValidation;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using WebForumApi.Application.Post.Tag;
using WebForumApi.Domain;

namespace WebForumApi.Test;

public sealed class TagPostHandlerTests
{
  private readonly ISessionManager sessionManager;
  private readonly IDataAccess dataAccess;
  private readonly IValidator<TagPostRequest> validator;
  private readonly TagPostHandler handler;

  public TagPostHandlerTests()
  {
    sessionManager = Substitute.For<ISessionManager>();
    dataAccess = Substitute.For<IDataAccess>();
    validator = Substitute.For<IValidator<TagPostRequest>>();

    handler = new TagPostHandler(sessionManager, dataAccess, validator);
  }

  [Fact]
  public async Task Handle_Success()
  {
    var request = new TagPostRequest
    {
      AccessToken = "token",
      PostGuid = Guid.NewGuid(),
      TagId = 1
    };

    var userGuid = Guid.NewGuid();

    sessionManager
      .GetCachedUser(request.AccessToken)
      .Returns(userGuid);

    var sut = handler.Handle(request, CancellationToken.None);

    Assert.True(sut.IsCompletedSuccessfully);

    await dataAccess
      .Received()
      .ExecuteProcedureAsync(
        StoredProcedures.TagPostProcedure,
        Arg.Any<Dictionary<string, object>>());
  }

  [Fact]
  public async Task Handle_Forbidden()
  {
    var request = new TagPostRequest
    {
      AccessToken = "token",
      PostGuid = Guid.NewGuid(),
      TagId = 1
    };

    var userGuid = Guid.NewGuid();

    sessionManager
      .GetCachedUser(request.AccessToken)
      .Returns(userGuid);

    dataAccess
      .ExecuteProcedureAsync(
        Arg.Any<string>(),
        Arg.Any<Dictionary<string, object>>())
      .Throws(new Exception("error"));

    await Assert.ThrowsAsync<SecurityException>(async () =>
      await handler.Handle(request, CancellationToken.None));
  }
}
