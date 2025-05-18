namespace WebForumApi.Test;

public sealed class SessionManagerData
{
  public static IEnumerable<object[]> ScenarioSuccess =>
  [
    [ Guid.NewGuid() ]
  ];

  public static IEnumerable<object[]> ScenarioUnauthorizedCache =>
  [
    [ null ],
    [ 1 ],
    [ Guid.Empty]
  ];

  public static IEnumerable<object[]> ScenarioUnauthorizedMissingToken =>
  [
    [ null ],
    [ string.Empty ]
  ];
}
