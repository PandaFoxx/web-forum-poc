using WebForumApi.Application.User.Login;

namespace WebForumApi.Test;

public class UserLoginHandlerTests
{
    private readonly UserLoginHandler handler;

    public UserLoginHandlerTests()
    {
        handler = new UserLoginHandler();
    }

    [Fact]
    public async Task Handle_Success()
    {
        var request = new UserLoginRequest
        {
            Username = "username",
            Password = "password"
        };

        var sut = await handler.Handle(request, CancellationToken.None);

        Assert.Null(sut.AccessToken);
    }
}
