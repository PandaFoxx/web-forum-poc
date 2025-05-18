using FluentValidation.TestHelper;
using WebForumApi.Application.Post.Tag;

namespace WebForumApi.Test;

public sealed class TagPostValidatorTests
{
  private readonly TagPostValidator validator;

  public TagPostValidatorTests()
  {
    validator = new TagPostValidator();
  }

  [Fact]
  public void Validation_Should_Pass()
  {
    var request = new TagPostRequest
    {
      PostGuid = Guid.NewGuid(),
      TagId = 1
    };

    var sut = validator.TestValidate(request);

    Assert.True(sut.IsValid);
  }

  [Fact]
  public void Validation_Should_Fail_When_Values_Invalid()
  {
    var request = new TagPostRequest
    {
      PostGuid = Guid.Empty,
      TagId = -1
    };

    var sut = validator.TestValidate(request);

    Assert.False(sut.IsValid);

    sut.ShouldHaveValidationErrorFor(x => x.PostGuid);
    sut.ShouldHaveValidationErrorFor(x => x.TagId);
  }

  [Fact]
  public void Validation_Should_Fail_When_Values_Empty()
  {
    var request = new TagPostRequest();

    var sut = validator.TestValidate(request);

    Assert.False(sut.IsValid);

    sut.ShouldHaveValidationErrorFor(x => x.PostGuid);
    sut.ShouldHaveValidationErrorFor(x => x.TagId);
  }
}
