using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace WebForumApi.Domain;

public static partial class TokenGenerator
{
  public static string Generate()
  {
    var token = new StringBuilder();

    for (var i = 0; i < 3; i++)
    {
      var byteArray = new byte[32];

      using var rng = RandomNumberGenerator.Create();

      rng.GetBytes(byteArray);

      var part = Convert.ToBase64String(byteArray);

      token.Append(CleanRegex().Replace(part, ""));
    }

    return token.ToString();
  }

  [GeneratedRegex(@"[^a-zA-Z0-9]")]
  private static partial Regex CleanRegex();
}
