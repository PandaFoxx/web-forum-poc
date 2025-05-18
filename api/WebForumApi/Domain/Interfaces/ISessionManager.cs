namespace WebForumApi.Domain;

public interface ISessionManager
{
  Guid GetCachedUser(string token);
}
