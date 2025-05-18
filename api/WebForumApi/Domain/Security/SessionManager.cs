using Microsoft.Extensions.Caching.Memory;

namespace WebForumApi.Domain;

public sealed class SessionManager(
  IMemoryCache cache
)
  : ISessionManager
{
  public Guid GetCachedUser(string token)
  {
    if (string.IsNullOrWhiteSpace(token))
      throw new UnauthorizedAccessException(Messages.MissingAccessToken);

    var cacheValue = cache.Get(token);

    if (!Guid.TryParse(cacheValue?.ToString(), out var userGuid) || userGuid == Guid.Empty)
      throw new UnauthorizedAccessException(Messages.UnauthorizedAccess);

    return userGuid;
  }
}
