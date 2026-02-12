using FluentValidation;
using Serilog;

namespace AgroSolutions.Identity.Application.Commands.UpdateUser;

public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        Log.Information("Starting the validator {ValidatorName}.", nameof(UpdateUserCommandValidator));

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(user => user.UserId)
            .GreaterThan(0).WithMessage("The user identifier is invalid");

        RuleFor(user => user.Name)
            .NotEmpty().WithMessage("The name needs to be provided")
            .MaximumLength(60).WithMessage("The name cannot exceed 60 characters");

        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("The email address needs to be provided")
            .MaximumLength(60).WithMessage("The email cannot exceed 60 characters")
            .EmailAddress().WithMessage("Please provide a valid email address");
    }
}
