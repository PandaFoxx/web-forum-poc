using System.Diagnostics.CodeAnalysis;
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

    builder.Services.AddScoped<IDataAccess, DataAccess>();

    builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection(DatabaseSettings.Position));

    var app = builder.Build();
    app.UseRouting();
    app.UseHttpsRedirection();
    app.MapControllers();
    app.Run();
  }
}