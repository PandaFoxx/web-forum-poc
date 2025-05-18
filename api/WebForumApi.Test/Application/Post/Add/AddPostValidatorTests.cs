using FluentValidation.TestHelper;
using WebForumApi.Application.Post.Add;

namespace WebForumApi.Test;

public sealed class AddPostValidatorTests
{
  private readonly AddPostValidator validator;

  public AddPostValidatorTests()
  {
    validator = new AddPostValidator();
  }

  [Fact]
  public void Validation_Should_Pass()
  {
    var request = new AddPostRequest
    {
      Title = "title",
      Content = "content"
    };

    var sut = validator.TestValidate(request);

    Assert.True(sut.IsValid);
  }

  [Fact]
  public void Validation_Should_Fail_When_Values_Too_Short()
  {
    var request = new AddPostRequest
    {
      Title = "",
      Content = ""
    };

    var sut = validator.TestValidate(request);

    Assert.False(sut.IsValid);

    sut.ShouldHaveValidationErrorFor(x => x.Title);
    sut.ShouldHaveValidationErrorFor(x => x.Content);
  }

  [Fact]
  public void Validation_Should_Fail_When_Values_Too_Long()
  {
    var request = new AddPostRequest
    {
      Title = "".PadRight(201),
      Content = "".PadRight(1001)
    };

    var sut = validator.TestValidate(request);

    Assert.False(sut.IsValid);

    sut.ShouldHaveValidationErrorFor(x => x.Title);
    sut.ShouldHaveValidationErrorFor(x => x.Content);
  }
}
