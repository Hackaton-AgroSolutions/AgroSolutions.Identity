using AgroSolutions.Identity.Application;
using FluentValidation;
using Serilog;

namespace AgroSolutions.Identity.Application.Commands.DeleteUser;

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        Log.Information("Starting the validator {ValidatorName}.", nameof(DeleteUserCommandValidator));

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(user => user.UserId)
            .GreaterThan(0).WithMessage("The user identifier is invalid");
    }
}
