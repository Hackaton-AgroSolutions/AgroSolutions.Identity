using FluentValidation;
using Serilog;

namespace AgroSolutions.Identity.Application.Queries.GetUser;

public class GetUserQueryValidator : AbstractValidator<GetUserQuery>
{
    public GetUserQueryValidator()
    {
        Log.Information("Starting the validator {ValidatorName}.", nameof(GetUserQueryValidator));

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(user => user.UserId)
            .GreaterThan(0).WithMessage("The user identifier is invalid");
    }
}
