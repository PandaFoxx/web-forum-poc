namespace WebForumApi.Domain;

public interface IDataAccess
{
  List<T> ExecuteProcedure<T>(string query, Dictionary<string, object> parameters) where T : new();
}
