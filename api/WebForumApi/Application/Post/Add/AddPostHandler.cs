using System.Security;
using FluentValidation;
using MediatR;
using WebForumApi.Domain;

namespace WebForumApi.Application.Post.Add;

public sealed class AddPostResponse
{
  public Guid PostGuid { get; set; }
}

public sealed class AddPostDto
{
  public Guid post_guid { get; set; }
}

public sealed class AddPostRequest
  : IRequest<AddPostResponse>
{
  public string AccessToken { get; set; }
  public string Title { get; set; }
  public string Content { get; set; }
}

public sealed class AddPostValidator
  : AbstractValidator<AddPostRequest>
{
  public AddPostValidator()
  {
    RuleFor(x => x.Title).NotNull().Length(1, 200);

    RuleFor(x => x.Content).NotNull().Length(1, 1000);
  }
}

public sealed class AddPostHandler(
  ISessionManager sessionManager,
  IDataAccess dataAccess,
  IValidator<AddPostRequest> validator
)
  : IRequestHandler<AddPostRequest, AddPostResponse>
{
  public async Task<AddPostResponse> Handle(
    AddPostRequest request,
    CancellationToken cancellationToken)
  {
    var userGuid = sessionManager.GetCachedUser(request.AccessToken);

    await validator.ValidateAndThrowAsync(request, cancellationToken);

    var parameters = new Dictionary<string, object>()
    {
      { nameof(userGuid), userGuid },
      { nameof(request.Title), request.Title },
      { nameof(request.Content), request.Content },
    };

    try
    {
      var result = await dataAccess.ExecuteProcedureAsync<AddPostDto>(
        StoredProcedures.AddPostProcedure, parameters);

      return new AddPostResponse
      {
        PostGuid = result.First().post_guid
      };
    }
    catch (Exception)
    {
      throw new SecurityException(Messages.ForbiddenError);
    }
  }
}