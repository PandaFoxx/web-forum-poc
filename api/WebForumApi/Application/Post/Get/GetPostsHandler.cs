using FluentValidation;
using MediatR;
using Microsoft.Extensions.Options;
using WebForumApi.Domain;

namespace WebForumApi.Application.Post.Get;

public sealed class GetPostsResponse
{
  public PagingResponse Paging { get; set; } = new();
  public List<PostsResponse> Posts { get; set; } = [];
}

public sealed class PagingResponse
{
  public int PageLimit { get; set; }
  public int PageSize { get; set; }
  public int PageNumber { get; set; }
}

public sealed class PostsResponse
{
  public Guid PostGuid { get; set; }
  public string PostTitle { get; set; }
  public string PostContent { get; set; }
  public DateTime PostDateCreated { get; set; }
  public Guid PostUserGuid { get; set; }
  public string PostUserName { get; set; }
  public int LikeCount { get; set; }
  public int TagId { get; set; }
  public string TagName { get; set; }
  public List<CommentsResponse> Comments { get; set; } = [];
}

public sealed class CommentsResponse
{
  public Guid CommentGuid { get; set; }
  public string CommentContent { get; set; }
  public DateTime CommentDateCreated { get; set; }
  public string CommentUserName { get; set; }
}

public sealed class GetPostsDto
{
  public Guid post_guid { get; set; }
  public string post_title { get; set; }
  public string post_content { get; set; }
  public string post_date_created { get; set; }
  public Guid post_user_guid { get; set; }
  public string post_user_name { get; set; }
  public int like_count { get; set; }
  public int tag_id { get; set; }
  public string tag_name { get; set; }
  public Guid post_comment_guid { get; set; }
  public string comment_content { get; set; }
  public string comment_date_created { get; set; }
  public string comment_user_name { get; set; }
  public int post_count { get; set; }
}

public sealed class GetPostsRequest
  : IRequest<GetPostsResponse>
{
  public Guid? FilterUserGuid { get; set; }
  public string FilterDateFrom { get; set; }
  public string FilterDateTo { get; set; }
  public int? FilterTagId { get; set; }
  public string SortColumn { get; set; }
  public string SortDirection { get; set; }
  public int? PageSize { get; set; }
  public int? PageNumber { get; set; }
}

public sealed class GetPostsValidator
  : AbstractValidator<GetPostsRequest>
{
  public GetPostsValidator()
  {
  }
}

public sealed class GetPostsHandler(
  IDataAccess dataAccess,
  IOptions<DatabaseSettings> databaseSettings,
  IOptions<PagingSettings> pagingSettings,
  IValidator<GetPostsRequest> validator
)
  : IRequestHandler<GetPostsRequest, GetPostsResponse>
{
  public async Task<GetPostsResponse> Handle(
    GetPostsRequest request,
    CancellationToken cancellationToken)
  {
    await validator.ValidateAndThrowAsync(request, cancellationToken);

    var parameters = new Dictionary<string, object>()
    {
      { nameof(databaseSettings.Value.Passphrase), databaseSettings.Value.Passphrase },
      { nameof(request.FilterUserGuid), request.FilterUserGuid },
      { nameof(request.FilterDateFrom), request.FilterDateFrom },
      { nameof(request.FilterDateTo), request.FilterDateTo },
      { nameof(request.FilterTagId), request.FilterTagId },
      { nameof(request.SortColumn), request.SortColumn },
      { nameof(request.SortDirection), request.SortDirection },
      { nameof(request.PageSize), request.PageSize ?? pagingSettings.Value.PageSize },
      { nameof(request.PageNumber), request.PageNumber ?? pagingSettings.Value.PageNumber },
    };

    var result = await dataAccess.ExecuteProcedureAsync<GetPostsDto>(
      StoredProcedures.GetPostsProcedure, parameters);

    return new GetPostsResponse
    {
      Paging = new PagingResponse
      {
        PageLimit = result.FirstOrDefault()?.post_count ?? 0,
        PageSize = request.PageSize ?? pagingSettings.Value.PageSize,
        PageNumber = request.PageNumber ?? pagingSettings.Value.PageNumber,
      },
      Posts = result
        .GroupBy(a => a.post_guid)
        .Select(b => new PostsResponse
        {
          PostGuid = b.Key,
          PostTitle = b.First().post_title,
          PostContent = b.First().post_content,
          PostDateCreated = DateTime.Parse(b.First().post_date_created),
          PostUserGuid = b.First().post_user_guid,
          PostUserName = b.First().post_user_name,
          LikeCount = b.First().like_count,
          TagId = b.First().tag_id,
          TagName = b.First().tag_name,
          Comments = b
            .Select(d => new CommentsResponse
            {
              CommentGuid = d.post_comment_guid,
              CommentContent = d.comment_content,
              CommentDateCreated = DateTime.Parse(d.comment_date_created),
              CommentUserName = d.comment_user_name,
            }).ToList()
        }).ToList()
    };
  }
}