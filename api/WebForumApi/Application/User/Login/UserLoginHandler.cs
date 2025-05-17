using MediatR;
using Microsoft.Extensions.Options;
using WebForumApi.Domain;

namespace WebForumApi.Application.User.Login;

public sealed class UserLoginResponse
{
  public string AccessToken { get; set; }
}

public sealed class UserLoginRequest
  : IRequest<UserLoginResponse>
{
  public string Username { get; set; }
  public string Password { get; set; }
}

public sealed class UserLoginDto
{
  public Guid UserGuid { get; set; }
}

public sealed class UserLoginHandler(
  IDataAccess dataAccess,
  IOptions<DatabaseSettings> databaseSettings
)
  : IRequestHandler<UserLoginRequest, UserLoginResponse>
{
  public async Task<UserLoginResponse> Handle(
    UserLoginRequest request,
    CancellationToken cancellationToken
  )
  {
    var parameters = new Dictionary<string, object>()
    {
      { nameof(databaseSettings.Value.Passphrase), databaseSettings.Value.Passphrase },
      { nameof(request.Username), request.Username },
      { nameof(request.Password), request.Password },
    };

    var result = await dataAccess.ExecuteProcedureAsync<UserLoginDto>(StoredProcedures.UserLoginProcedure, parameters);

    if ((result?.Count ?? 0) == 0)
    {
      throw new UnauthorizedAccessException(Messages.UnauthorizedAccess);
    }

    var accessToken = TokenGenerator.Generate();

    return new UserLoginResponse
    {
      AccessToken = accessToken
    };
  }
}
