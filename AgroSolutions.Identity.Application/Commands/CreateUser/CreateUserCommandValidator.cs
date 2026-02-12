using FluentValidation;
using Serilog;

namespace AgroSolutions.Identity.Application.Commands.CreateUser;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        Log.Information("Starting the validator {ValidatorName}.", nameof(CreateUserCommandValidator));

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(user => user.Name)
            .NotEmpty().WithMessage("The name needs to be provided")
            .MaximumLength(60).WithMessage("The name cannot exceed 60 characters");

        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("The email address needs to be provided")
            .MaximumLength(60).WithMessage("The email cannot exceed 60 characters")
            .EmailAddress().WithMessage("Please provide a valid email address");

        RuleFor(user => user.Password)
            .NotEmpty().WithMessage("The password must be entered")
            .Matches(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[^\w\s]).{8,}$")
            .WithMessage("The password must be at least 8 characters long and contain letters, numbers, and special characters");
    }
}
