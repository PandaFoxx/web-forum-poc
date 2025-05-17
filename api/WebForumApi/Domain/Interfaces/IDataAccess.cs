namespace WebForumApi.Domain;

public interface IDataAccess
{
  List<T> SelectMultipleRecords<T>(string query, Dictionary<string, object> parameters) where T : new();
}
