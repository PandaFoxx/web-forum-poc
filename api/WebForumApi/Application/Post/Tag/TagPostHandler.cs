using System.Security;
using FluentValidation;
using MediatR;
using WebForumApi.Domain;

namespace WebForumApi.Application.Post.Tag;

public sealed class TagPostRequest
  : IRequest
{
  public string AccessToken { get; set; }
  public Guid PostGuid { get; set; }
  public int TagId { get; set; }
}

public sealed class TagPostValidator
  : AbstractValidator<TagPostRequest>
{
  public TagPostValidator()
  {
    RuleFor(x => x.PostGuid).NotEmpty();

    RuleFor(x => x.TagId).GreaterThan(0);
  }
}

public sealed class TagPostHandler(
  ISessionManager sessionManager,
  IDataAccess dataAccess,
  IValidator<TagPostRequest> validator
)
  : IRequestHandler<TagPostRequest>
{
  public async Task Handle(
    TagPostRequest request,
    CancellationToken cancellationToken)
  {
    var userGuid = sessionManager.GetCachedUser(request.AccessToken);

    await validator.ValidateAndThrowAsync(request, cancellationToken);

    var parameters = new Dictionary<string, object>()
    {
      { nameof(userGuid), userGuid },
      { nameof(request.PostGuid), request.PostGuid },
      { nameof(request.TagId), request.TagId },
    };

    try
    {
      await dataAccess.ExecuteProcedureAsync(
        StoredProcedures.TagPostProcedure, parameters);
    }
    catch (Exception)
    {
      throw new SecurityException(Messages.ForbiddenError);
    }
  }
}