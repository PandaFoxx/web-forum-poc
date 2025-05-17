using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using WebForumApi.Domain;

namespace WebForumApi.Data;

[ExcludeFromCodeCoverage]
public class DataAccess(
  ILogger<DataAccess> logger,
  IOptions<DatabaseSettings> databaseSettings
)
  : IDataAccess
{
  public List<T> ExecuteProcedure<T>(
    string query,
    Dictionary<string, object> parameters
  )
    where T : new()
  {
    try
    {
      var result = new List<T>();

      using var connection = new SqlConnection(databaseSettings.Value.ConnectionString);
      using var command = new SqlCommand(query, connection);

      command.CommandType = CommandType.StoredProcedure;

      if (parameters is not null)
      {
        foreach (var parameter in parameters)
        {
          command.Parameters.AddWithValue(parameter.Key, parameter.Value ?? DBNull.Value);
        }
      }

      connection.Open();

      using var reader = command.ExecuteReader();

      var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

      var columnNames = new HashSet<string>(
        Enumerable.Range(0, reader.FieldCount).Select(reader.GetName),
        StringComparer.OrdinalIgnoreCase
      );

      for (var i = 0; i < reader.FieldCount; i++)
      {
        columnNames.Add(reader.GetName(i));
      }

      while (reader.Read())
      {
        var item = new T();

        foreach (var property in properties)
        {
          if (columnNames.Contains(property.Name) && !reader.IsDBNull(reader.GetOrdinal(property.Name)))
          {
            var columnValue = reader.GetValue(reader.GetOrdinal(property.Name));
            property.SetValue(item, Convert.ChangeType(columnValue, property.PropertyType));
          }
        }

        result.Add(item);
      }

      return result;
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Unable to execute: {StoredProcedure}", query);
      throw;
    }
  }
}
