using FluentValidation;
using Microsoft.Extensions.Options;
using NSubstitute;
using WebForumApi.Application.Post.Get;
using WebForumApi.Domain;

namespace WebForumApi.Test;

public sealed class GetPostsHandlerTests
{
  private readonly IDataAccess dataAccess;
  private readonly IOptions<DatabaseSettings> databaseSettings;
  private readonly IOptions<PagingSettings> pagingSettings;
  private readonly IValidator<GetPostsRequest> validator;
  private readonly GetPostsHandler handler;

  public GetPostsHandlerTests()
  {
    dataAccess = Substitute.For<IDataAccess>();
    validator = Substitute.For<IValidator<GetPostsRequest>>();

    databaseSettings = Options.Create(new DatabaseSettings
    {
      Passphrase = "passphrase"
    });

    pagingSettings = Options.Create(new PagingSettings
    {
      PageNumber = 1,
      PageSize = 10
    });

    handler = new GetPostsHandler(dataAccess, databaseSettings, pagingSettings, validator);
  }

  [Fact]
  public async Task Handle_Success()
  {
    var request = new GetPostsRequest
    {
      FilterUserGuid = Guid.NewGuid(),
      FilterDateFrom = DateTime.UtcNow.ToString("yyyy-MM-dd"),
      FilterDateTo = DateTime.UtcNow.ToString("yyyy-MM-dd"),
      FilterTagId = 1,
      SortColumn = "post_date",
      SortDirection = "DESC",
      PageSize = 10,
      PageNumber = 1
    };

    var dto = new List<GetPostsDto>
    {
      new ()
      {
        post_guid = Guid.NewGuid(),
        post_title = "post_title",
        post_content = "post_content",
        post_date_created = DateTime.UtcNow.ToString("yyyy-MM-dd"),
        post_user_guid = Guid.NewGuid(),
        post_user_name = "post_user_name",
        like_count = 7,
        tag_id = 1,
        tag_name = "misinformation",
        post_comment_guid = Guid.NewGuid(),
        comment_content = "comment_content",
        comment_date_created = DateTime.UtcNow.ToString("yyyy-MM-dd"),
        comment_user_name = "comment_user_name",
        post_count = 100,
      }
    };

    var response = new GetPostsResponse
    {
      Paging = new PagingResponse
      {
        PageLimit = 100,
        PageSize = 10,
        PageNumber = 1,
      },
      Posts = new List<PostsResponse>
      {
        new()
        {
          PostGuid = dto.First().post_guid,
          PostTitle = dto.First().post_title,
          PostContent = dto.First().post_content,
          PostDateCreated = DateTime.Parse(dto.First().post_date_created),
          PostUserGuid = dto.First().post_user_guid,
          PostUserName = dto.First().post_user_name,
          LikeCount = dto.First().like_count,
          TagId = dto.First().tag_id,
          TagName = dto.First().tag_name,
          Comments = new List<CommentsResponse>
          {
            new()
            {
              CommentGuid = dto.First().post_comment_guid,
              CommentContent = dto.First().comment_content,
              CommentDateCreated = DateTime.Parse(dto.First().comment_date_created),
              CommentUserName = dto.First().comment_user_name,
            }
          }
        }
      }
    };

    dataAccess
      .ExecuteProcedureAsync<GetPostsDto>(
        Arg.Any<string>(),
        Arg.Any<Dictionary<string, object>>())
      .Returns(dto);

    var sut = await handler.Handle(request, CancellationToken.None);

    Assert.Equivalent(response, sut);
  }
}
