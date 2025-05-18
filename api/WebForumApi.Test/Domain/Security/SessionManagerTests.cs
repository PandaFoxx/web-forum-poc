using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using WebForumApi.Domain;

namespace WebForumApi.Test;

public sealed class SessionManagerTests
{
  private readonly IMemoryCache cache;
  private readonly SessionManager sessionManager;

  public SessionManagerTests()
  {
    cache = Substitute.For<IMemoryCache>();
    sessionManager = new SessionManager(cache);
  }

  [Theory]
  [MemberData(
    nameof(SessionManagerData.ScenarioSuccess),
    MemberType = typeof(SessionManagerData))]
  public void GetCachedUser_Success(
    Guid cacheValue
  )
  {
    var token = "token";

    cache
      .TryGetValue(token, out Arg.Any<Object>())
      .Returns(call =>
      {
        call[1] = cacheValue;
        return true;
      });

    var sut = sessionManager.GetCachedUser(token);

    Assert.Equal(cacheValue, sut);
  }

  [Theory]
  [MemberData(
    nameof(SessionManagerData.ScenarioUnauthorizedCache),
    MemberType = typeof(SessionManagerData))]
  public void GetCachedUser_Unauthorized_When_Missing_Or_Invalid_Cache(
    object cacheValue
  )
  {
    var token = "token";

    cache
      .TryGetValue(Arg.Any<string>(), out Arg.Any<Object>())
      .Returns(call =>
      {
        call[1] = cacheValue;
        return true;
      });

    Assert.Throws<UnauthorizedAccessException>(()
      => sessionManager.GetCachedUser(token));
  }

  [Theory]
  [MemberData(
    nameof(SessionManagerData.ScenarioUnauthorizedMissingToken),
    MemberType = typeof(SessionManagerData))]
  public void GetCachedUser_Unauthorized_When_Token_Missing(
    string token
  )
  {
    Assert.Throws<UnauthorizedAccessException>(()
      => sessionManager.GetCachedUser(token));
  }
}
