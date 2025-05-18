using System.Security;
using FluentValidation;
using MediatR;
using WebForumApi.Domain;

namespace WebForumApi.Application.Post.Like;

public sealed class LikePostResponse
{
  public int PostLikeId { get; set; }
}

public sealed class LikePostDto
{
  public int post_like_id { get; set; }
}

public sealed class LikePostRequest
  : IRequest<LikePostResponse>
{
  public string AccessToken { get; set; }
  public Guid PostGuid { get; set; }
  public bool? Unlike { get; set; } = false; // default to false if not provided
}

public sealed class LikePostValidator
  : AbstractValidator<LikePostRequest>
{
  public LikePostValidator()
  {
    RuleFor(x => x.PostGuid).NotEmpty();
  }
}

public sealed class LikePostHandler(
  ISessionManager sessionManager,
  IDataAccess dataAccess,
  IValidator<LikePostRequest> validator
)
  : IRequestHandler<LikePostRequest, LikePostResponse>
{
  public async Task<LikePostResponse> Handle(
    LikePostRequest request,
    CancellationToken cancellationToken)
  {
    var userGuid = sessionManager.GetCachedUser(request.AccessToken);

    await validator.ValidateAndThrowAsync(request, cancellationToken);

    var parameters = new Dictionary<string, object>()
    {
      { nameof(userGuid), userGuid },
      { nameof(request.PostGuid), request.PostGuid },
      { nameof(request.Unlike), request.Unlike },
    };

    try
    {
      var result = await dataAccess.ExecuteProcedureAsync<LikePostDto>(
        StoredProcedures.LikePostProcedure, parameters);

      return new LikePostResponse
      {
        PostLikeId = result.First().post_like_id
      };
    }
    catch (Exception)
    {
      throw new SecurityException(Messages.ForbiddenError);
    }
  }
}