namespace WebForumApi.Domain;

public sealed class PagingSettings
{
  public const string Position = "Paging";
  public int PageSize { get; set; }
  public int PageNumber { get; set; }
}
