using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
internal class Program
{
  private static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddMediatR(config =>
      config.RegisterServicesFromAssembly(typeof(Program).Assembly)
    );

    builder.Services.AddControllers();

    var app = builder.Build();
    app.UseRouting();
    app.UseHttpsRedirection();
    app.MapControllers();
    app.Run();
  }
}