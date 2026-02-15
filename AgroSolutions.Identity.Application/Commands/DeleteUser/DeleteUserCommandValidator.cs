using AgroSolutions.Identity.Application.Validators;
using FluentValidation;
using Serilog;

namespace AgroSolutions.Identity.Application.Commands.DeleteUser;

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        Log.Information("Starting the validator {ValidatorName}.", nameof(DeleteUserCommandValidator));

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(c => c.UserId).ValidUserId();
    }
}
