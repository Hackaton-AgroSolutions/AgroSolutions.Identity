using AgroSolutions.Identity.Application.Queries.GetUserById;
using FluentAssertions;
using FluentValidation.Results;

namespace AgroSolutions.Identity.Tests.Validators;

public class GetUserByIdQueryTests
{
    [Fact(DisplayName = "Valid query should pass validation")]
    public void Should_BeValid_WhenQueryIsValid()
    {
        // Arrange
        GetUserByIdQuery getUserByIdQuery = new(1);

        // Act
        ValidationResult result = new GetUserByIdQueryValidator().Validate(getUserByIdQuery);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact(DisplayName = "Invalid query should fail validation")]
    public void Should_BeInvalid_WhenQueryIsInvalid()
    {
        // Arrange
        GetUserByIdQuery getUserByIdQuery = new(-21);

        // Act
        ValidationResult result = new GetUserByIdQueryValidator().Validate(getUserByIdQuery);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
    }
}