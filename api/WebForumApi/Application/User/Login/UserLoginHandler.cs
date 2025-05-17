using MediatR;

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

public sealed class UserLoginHandler
  : IRequestHandler<UserLoginRequest, UserLoginResponse>
{
  public async Task<UserLoginResponse> Handle(
    UserLoginRequest request,
    CancellationToken cancellationToken
  )
  {
    return await Task.FromResult(new UserLoginResponse());
  }
}
