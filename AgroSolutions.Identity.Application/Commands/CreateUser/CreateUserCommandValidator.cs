using AgroSolutions.Identity.Application.Validators;
using FluentValidation;
using Serilog;

namespace AgroSolutions.Identity.Application.Commands.CreateUser;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        Log.Information("Starting the validator {ValidatorName}.", nameof(CreateUserCommandValidator));

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(c => c.Name).ValidUserName();
        RuleFor(c => c.Email).ValidUserEmail();
        RuleFor(c => c.Password).ValidUserPassword();
    }
}
