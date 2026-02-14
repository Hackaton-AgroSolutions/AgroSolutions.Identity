using FluentValidation;
using Serilog;

namespace AgroSolutions.Identity.Application.Queries.GetUserByEmailAndPassword;

public class GetUserByEmailAndPasswordQueryValidator : AbstractValidator<GetUserByEmailAndPasswordQuery>
{
    public GetUserByEmailAndPasswordQueryValidator()
    {
        Log.Information("Starting the validator {ValidatorName}.", nameof(GetUserByEmailAndPasswordQueryValidator));

        RuleLevelCascadeMode = CascadeMode.Stop;

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
