namespace WebForumApi.Domain;

public sealed class DatabaseSettings
{
  public const string Position = "Database";
  public string ConnectionString { get; set; }
  public string Passphrase { get; set; }
}
