using System.Security;
using FluentValidation;
using MediatR;
using WebForumApi.Domain;

namespace WebForumApi.Application.Post.Comment;

public sealed class AddPostCommentResponse
{
  public Guid PostCommentGuid { get; set; }
}

public sealed class AddPostCommentDto
{
  public Guid post_comment_guid { get; set; }
}

public sealed class AddPostCommentRequest
  : IRequest<AddPostCommentResponse>
{
  public string AccessToken { get; set; }
  public Guid PostGuid { get; set; }
  public string Content { get; set; }
}

public sealed class AddPostCommentValidator
  : AbstractValidator<AddPostCommentRequest>
{
  public AddPostCommentValidator()
  {
    RuleFor(x => x.PostGuid).NotEmpty();

    RuleFor(x => x.Content).NotNull().Length(1, 1000);
  }
}

public sealed class AddPostCommentHandler(
  ISessionManager sessionManager,
  IDataAccess dataAccess,
  IValidator<AddPostCommentRequest> validator
)
  : IRequestHandler<AddPostCommentRequest, AddPostCommentResponse>
{
  public async Task<AddPostCommentResponse> Handle(
    AddPostCommentRequest request,
    CancellationToken cancellationToken)
  {
    var userGuid = sessionManager.GetCachedUser(request.AccessToken);

    await validator.ValidateAndThrowAsync(request, cancellationToken);

    var parameters = new Dictionary<string, object>()
    {
      { nameof(userGuid), userGuid },
      { nameof(request.PostGuid), request.PostGuid },
      { nameof(request.Content), request.Content },
    };

    try
    {
      var result = await dataAccess.ExecuteProcedureAsync<AddPostCommentDto>(
        StoredProcedures.AddPostCommentProcedure, parameters);

      return new AddPostCommentResponse
      {
        PostCommentGuid = result.First().post_comment_guid
      };
    }
    catch (Exception)
    {
      throw new SecurityException(Messages.ForbiddenError);
    }
  }
}