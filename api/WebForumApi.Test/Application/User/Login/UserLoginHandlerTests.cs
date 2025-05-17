using Microsoft.Extensions.Options;
using NSubstitute;
using WebForumApi.Application.User.Login;
using WebForumApi.Domain;

namespace WebForumApi.Test;

public class UserLoginHandlerTests
{
    private readonly IDataAccess dataAccess;
    private readonly IOptions<DatabaseSettings> databaseSettings;
    private readonly UserLoginHandler handler;

    public UserLoginHandlerTests()
    {
        dataAccess = Substitute.For<IDataAccess>();

        databaseSettings = Options.Create(new DatabaseSettings
        {
            ConnectionString = "connection_string",
            Passphrase = "pass_phrase"
        });

        handler = new UserLoginHandler(dataAccess, databaseSettings);
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
