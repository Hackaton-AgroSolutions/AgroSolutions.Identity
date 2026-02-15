using AgroSolutions.Identity.Application.Queries.GetUser;
using AgroSolutions.Identity.Application.Validators;
using FluentValidation;
using Serilog;

namespace AgroSolutions.Identity.Application.Queries.ValidateToken;

public class ValidateTokenQueryValidator : AbstractValidator<ValidateTokenQuery>
{
    public ValidateTokenQueryValidator()
    {
        Log.Information("Starting the validator {ValidatorName}.", nameof(GetUserQueryValidator));

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(q => q.UserId).ValidUserId();
    }
}
