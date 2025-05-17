using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebForumApi.Controllers;

[ExcludeFromCodeCoverage]
public sealed class ErrorResponse : ObjectResult
{
  public ErrorResponse(object value)
    : base(value)
  {
    StatusCode = StatusCodes.Status500InternalServerError;
  }
}

[ExcludeFromCodeCoverage]
public sealed class ErrorHandlerAttribute(
  ILogger<ErrorHandlerAttribute> logger
)
  : ExceptionFilterAttribute
{
  public override void OnException(ExceptionContext context)
  {
    if (context.ExceptionHandled)
      return;

    logger.LogError(context.Exception, "An unhandled exception occurred: {Message}", context.Exception.Message);

    var errorResponse = new
    {
      Error = context.Exception.Message
    };

    context.Result = context.Exception switch
    {
      MissingFieldException => new NotFoundObjectResult(errorResponse),
      UnauthorizedAccessException => new UnauthorizedObjectResult(errorResponse),
      ArgumentNullException => new BadRequestObjectResult(errorResponse),
      ArgumentException => new BadRequestObjectResult(errorResponse),
      ValidationException => new BadRequestObjectResult(errorResponse),
      _ => new ErrorResponse(errorResponse)
    };

    context.ExceptionHandled = true;
  }
}