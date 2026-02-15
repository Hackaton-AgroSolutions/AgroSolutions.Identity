using AgroSolutions.Identity.Application.Validators;
using FluentValidation;
using Serilog;

namespace AgroSolutions.Identity.Application.Commands.UpdateUser;

public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        Log.Information("Starting the validator {ValidatorName}.", nameof(UpdateUserCommandValidator));

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(c => c.UserId).ValidUserId();
        RuleFor(c => c.Name).ValidUserName();
        RuleFor(c => c.Email).ValidUserEmail();
    }
}
