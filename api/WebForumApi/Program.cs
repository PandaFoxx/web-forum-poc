using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using WebForumApi.Application.Post.Add;
using WebForumApi.Application.Post.Comment;
using WebForumApi.Application.Post.Like;
using WebForumApi.Controllers;
using WebForumApi.Data;
using WebForumApi.Domain;

[ExcludeFromCodeCoverage]
internal class Program
{
  private static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddMediatR(config =>
      config.RegisterServicesFromAssembly(typeof(Program).Assembly)
    );

    builder.Services.AddControllers(options =>
      options.Filters.Add<ErrorHandlerAttribute>()
    );

    // Register services
    builder.Services.AddMemoryCache();
    builder.Services.AddTransient<IDataAccess, DataAccess>();
    builder.Services.AddTransient<ISessionManager, SessionManager>();

    // Register validators
    builder.Services.AddTransient<IValidator<AddPostRequest>, AddPostValidator>();
    builder.Services.AddTransient<IValidator<AddPostCommentRequest>, AddPostCommentValidator>();
    builder.Services.AddTransient<IValidator<LikePostRequest>, LikePostValidator>();

    // Register settings
    builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection(DatabaseSettings.Position));

    var app = builder.Build();
    app.UseRouting();
    app.UseHttpsRedirection();
    app.MapControllers();
    app.Run();
  }
}