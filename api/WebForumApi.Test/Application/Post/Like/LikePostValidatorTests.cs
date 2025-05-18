using FluentValidation.TestHelper;
using WebForumApi.Application.Post.Like;

namespace WebForumApi.Test;

public sealed class LikePostValidatorTests
{
  private readonly LikePostValidator validator;

  public LikePostValidatorTests()
  {
    validator = new LikePostValidator();
  }

  [Fact]
  public void Validation_Should_Pass()
  {
    var request = new LikePostRequest
    {
      PostGuid = Guid.NewGuid()
    };

    var sut = validator.TestValidate(request);

    Assert.True(sut.IsValid);
  }

  [Fact]
  public void Validation_Should_Fail_When_Values_Empty()
  {
    var request = new LikePostRequest();

    var sut = validator.TestValidate(request);

    Assert.False(sut.IsValid);

    sut.ShouldHaveValidationErrorFor(x => x.PostGuid);
  }
}
