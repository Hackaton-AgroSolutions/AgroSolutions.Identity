using AgroSolutions.Identity.Application.Commands.CreateUser;
using AgroSolutions.Identity.Application.Commands.UpdateUser;
using AgroSolutions.Identity.Application.Validators;
using FluentAssertions;
using FluentValidation.Results;

namespace AgroSolutions.Identity.Tests.Validators;

public class CreateUserCommandValidatorTests
{
    [Fact(DisplayName = "Valid command should pass validation")]
    public void Should_BeValid_WhenCommandIsValid()
    {
        // Arrange
        CreateUserCommand createUserCommand = new("Valid Name User", "validEmail@gmail.com", "F4hHW#7G40,!");

        // Act
        ValidationResult result = new CreateUserCommandValidator().Validate(createUserCommand);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact(DisplayName = "Invalid command should fail validation")]
    public void Should_BeInvalid_WhenCommandIsInvalid()
    {
        // Arrange
        CreateUserCommand createUserCommand = new("Invalid name with more than 60 characters, for unit testing purposes only", "invalidEmail", "invalidPassword");

        // Act
        ValidationResult result = new CreateUserCommandValidator().Validate(createUserCommand);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(3);
        result.Errors.Count(e => e.ErrorMessage == UserFieldsValidationExtensions.MESSAGE_INVALID_LENGTH_USERNAME).Should().Be(1);
        result.Errors.Count(e => e.ErrorMessage == UserFieldsValidationExtensions.MESSAGE_INVALID_USEREMAIL).Should().Be(1);
        result.Errors.Count(e => e.ErrorMessage == UserFieldsValidationExtensions.MESSAGE_INVALID_USERPASSWORD).Should().Be(1);
    }

    [Fact(DisplayName = "Invalid email should return email validation error")]
    public void Should_HaveEmailError_WhenEmailIsInvalid()
    {
        // Arrange
        CreateUserCommand createUserCommand = new("Valid Name User", "invalidEmail", "F4hHW#7G40,!");

        // Act
        ValidationResult result = new CreateUserCommandValidator().Validate(createUserCommand);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors.Count(e => e.ErrorMessage == UserFieldsValidationExtensions.MESSAGE_INVALID_USEREMAIL).Should().Be(1);
    }
}
