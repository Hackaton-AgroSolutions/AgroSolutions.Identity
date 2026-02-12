using FluentValidation;
using Serilog;

namespace AgroSolutions.Identity.Application.Queries.GetUserById;

public class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUserByIdQueryValidator()
    {
        Log.Information("Starting the validator {ValidatorName}.", nameof(GetUserByIdQueryValidator));

        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(user => user.UserId)
            .GreaterThan(0).WithMessage("The user identifier is invalid");
    }
}
