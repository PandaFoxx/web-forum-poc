using System.Security;
using Microsoft.Extensions.Caching.Memory;

namespace WebForumApi.Domain;

public sealed class SessionManager(
  IMemoryCache cache
)
  : ISessionManager
{
  public Guid GetCachedUser(string token)
  {
    var cacheValue = cache.Get(token);

    if (!Guid.TryParse(cacheValue?.ToString(), out var userGuid) || userGuid == Guid.Empty)
    {
      throw new SecurityException(Messages.ForbiddenError);
    }

    return userGuid;
  }
}
