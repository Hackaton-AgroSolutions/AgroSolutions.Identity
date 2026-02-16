using AgroSolutions.Identity.Application.Commands.UpdateUser;
using AgroSolutions.Identity.Application.Validators;
using FluentAssertions;
using FluentValidation.Results;

namespace AgroSolutions.Identity.Tests.Validators;

public class UpdateUserCommandValidatorTests
{
    [Fact(DisplayName = "Valid command should pass validation")]
    public void Should_BeValid_WhenCommandIsValid()
    {
        // Arrange
        UpdateUserCommand updateUserCommand = new(1, "New name user", "newValidEmail@gmail.com");

        // Act
        ValidationResult result = new UpdateUserCommandValidator().Validate(updateUserCommand);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact(DisplayName = "Invalid command should fail validation")]
    public void Should_BeInvalid_WhenCommandIsInvalid()
    {
        // Arrange
        UpdateUserCommand updateUserCommand = new(0, "Invalid name with more than 60 characters, for unit testing purposes only", "invalidEmail");

        // Act
        ValidationResult result = new UpdateUserCommandValidator().Validate(updateUserCommand);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(3);
        result.Errors.Count(e => e.ErrorMessage == UserFieldsValidationExtensions.MESSAGE_INVALID_USERID).Should().Be(1);
        result.Errors.Count(e => e.ErrorMessage == UserFieldsValidationExtensions.MESSAGE_INVALID_LENGTH_USERNAME).Should().Be(1);
        result.Errors.Count(e => e.ErrorMessage == UserFieldsValidationExtensions.MESSAGE_INVALID_USEREMAIL).Should().Be(1);
    }

    [Fact(DisplayName = "Invalid user Id should return user id validation error")]
    public void Should_HaveUserIdError_WhenUserIdIsInvalid()
    {
        // Arrange
        UpdateUserCommand updateUserCommand = new(-23, "New name user", "newValidEmail@gmail.com");

        // Act
        ValidationResult result = new UpdateUserCommandValidator().Validate(updateUserCommand);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors.Count(e => e.ErrorMessage == UserFieldsValidationExtensions.MESSAGE_INVALID_USERID).Should().Be(1);
    }
}
