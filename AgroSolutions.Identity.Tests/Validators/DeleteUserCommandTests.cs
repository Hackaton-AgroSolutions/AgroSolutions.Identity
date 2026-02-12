using AgroSolutions.Identity.Application.Commands.DeleteUser;
using FluentAssertions;
using FluentValidation.Results;

namespace AgroSolutions.Identity.Tests.Validators;

public class DeleteUserCommandTests
{
    [Fact(DisplayName = "Valid command should pass validation")]
    public void Should_BeValid_WhenCommandIsValid()
    {
        // Arrange
        DeleteUserCommand createUserCommand = new(1);

        // Act
        ValidationResult result = new DeleteUserCommandValidator().Validate(createUserCommand);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact(DisplayName = "Invalid command should fail validation")]
    public void Should_BeInvalid_WhenCommandIsInvalid()
    {
        // Arrange
        DeleteUserCommand createUserCommand = new(-12);

        // Act
        ValidationResult result = new DeleteUserCommandValidator().Validate(createUserCommand);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Where(e => e.PropertyName != nameof(DeleteUserCommand.UserId)).Should().BeEmpty();
        result.Errors.Where(e => e.PropertyName == nameof(DeleteUserCommand.UserId)).Should().HaveCount(1);
    }
}
