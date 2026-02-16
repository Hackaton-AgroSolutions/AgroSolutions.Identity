using AgroSolutions.Identity.Application.Queries.AuthenticateUser;
using AgroSolutions.Identity.Application.Validators;
using FluentAssertions;
using FluentValidation.Results;

namespace AgroSolutions.Identity.Tests.Validators;

public class AuthenticateUserQueryValidatorTests
{
    [Fact(DisplayName = "Valid query should pass validation")]
    public void Should_BeValid_WhenQueryIsValid()
    {
        // Arrange
        AuthenticateUserQuery getUserByEmailAndPasswordQuery = new("validEmail@gmail.com", "password1234$$");

        // Act
        ValidationResult result = new AuthenticateUserQueryValidator().Validate(getUserByEmailAndPasswordQuery);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact(DisplayName = "Invalid query should fail validation")]
    public void Should_BeInvalid_WhenQueryIsInvalid()
    {
        // Arrange
        AuthenticateUserQuery getUserByEmailAndPasswordQuery = new("invalidEmail", "invalidPassword");

        // Act
        ValidationResult result = new AuthenticateUserQueryValidator().Validate(getUserByEmailAndPasswordQuery);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
        result.Errors.Count(e => e.ErrorMessage == UserFieldsValidationExtensions.MESSAGE_INVALID_USEREMAIL).Should().Be(1);
        result.Errors.Count(e => e.ErrorMessage == UserFieldsValidationExtensions.MESSAGE_INVALID_USERPASSWORD).Should().Be(1);
    }

    [Fact(DisplayName = "Invalid password should return password validation error")]
    public void Should_HavePasswordError_WhenPasswordIsInvalid()
    {
        // Arrange
        AuthenticateUserQuery getUserByEmailAndPasswordQuery = new("validEmail@gmail.com", "invalidPassword");

        // Act
        ValidationResult result = new AuthenticateUserQueryValidator().Validate(getUserByEmailAndPasswordQuery);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors.Count(e => e.ErrorMessage == UserFieldsValidationExtensions.MESSAGE_INVALID_USERPASSWORD).Should().Be(1);
    }
}