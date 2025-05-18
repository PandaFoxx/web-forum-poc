namespace WebForumApi.Domain;

public interface IDataAccess
{
  Task<List<T>> ExecuteProcedureAsync<T>(string query, Dictionary<string, object> parameters) where T : new();

  Task ExecuteProcedureAsync(string query, Dictionary<string, object> parameters);
}
