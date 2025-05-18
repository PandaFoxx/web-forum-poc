namespace WebForumApi.Test;

public sealed class SessionManagerData
{
  public static IEnumerable<object[]> ScenarioSuccess =>
  [
    [ Guid.NewGuid() ]
  ];

  public static IEnumerable<object[]> ScenarioForbidden =>
  [
    [ null ],
    [ 1 ],
    [ Guid.Empty]
  ];
}
