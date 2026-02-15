using AgroSolutions.Identity.Application.Validators;
using FluentValidation;
using Serilog;

namespace AgroSolutions.Identity.Application.Queries.AuthenticateUser;

public class AuthenticateUserQueryValidator : AbstractValidator<AuthenticateUserQuery>
{
    public AuthenticateUserQueryValidator()
    {
        Log.Information("Starting the validator {ValidatorName}.", nameof(AuthenticateUserQueryValidator));

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(q => q.Email).ValidUserEmail();
        RuleFor(q => q.Password).ValidUserPassword();
    }
}
