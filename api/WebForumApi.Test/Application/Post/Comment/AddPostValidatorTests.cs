using FluentValidation.TestHelper;
using WebForumApi.Application.Post.Comment;

namespace WebForumApi.Test;

public sealed class AddPostCommentValidatorTests
{
  private readonly AddPostCommentValidator validator;

  public AddPostCommentValidatorTests()
  {
    validator = new AddPostCommentValidator();
  }

  [Fact]
  public void Validation_Should_Pass()
  {
    var request = new AddPostCommentRequest
    {
      PostGuid = Guid.NewGuid(),
      Content = "content"
    };

    var sut = validator.TestValidate(request);

    Assert.True(sut.IsValid);
  }

  [Fact]
  public void Validation_Should_Fail_When_Values_Empty()
  {
    var request = new AddPostCommentRequest();

    var sut = validator.TestValidate(request);

    Assert.False(sut.IsValid);

    sut.ShouldHaveValidationErrorFor(x => x.PostGuid);
    sut.ShouldHaveValidationErrorFor(x => x.Content);
  }

  [Fact]
  public void Validation_Should_Fail_When_Values_Too_Short()
  {
    var request = new AddPostCommentRequest
    {
      PostGuid = Guid.Empty,
      Content = ""
    };

    var sut = validator.TestValidate(request);

    Assert.False(sut.IsValid);

    sut.ShouldHaveValidationErrorFor(x => x.PostGuid);
    sut.ShouldHaveValidationErrorFor(x => x.Content);
  }

  [Fact]
  public void Validation_Should_Fail_When_Values_Too_Long()
  {
    var request = new AddPostCommentRequest
    {
      PostGuid = Guid.NewGuid(),
      Content = "".PadRight(1001)
    };

    var sut = validator.TestValidate(request);

    Assert.False(sut.IsValid);

    sut.ShouldHaveValidationErrorFor(x => x.Content);
  }
}
