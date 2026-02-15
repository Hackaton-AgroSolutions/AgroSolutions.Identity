using AgroSolutions.Identity.Application.Commands.UpdateUser;
using AgroSolutions.Identity.Domain.Entities;
using FluentValidation;
using System.Data;

namespace AgroSolutions.Identity.Application.Validators;

public static class UserValidationExtensions
{
    extension<T>(IRuleBuilder<T, int> rule)
    {
        public IRuleBuilderOptions<T, int> ValidUserId() => rule
            .GreaterThan(0).WithMessage("The user identifier is invalid");
    }

    extension<T>(IRuleBuilder<T, string> rule)
    {
        public IRuleBuilderOptions<T, string> ValidUserName() => rule
            .NotEmpty().WithMessage("The name needs to be provided")
            .MaximumLength(60).WithMessage("The name cannot exceed 60 characters");

        public IRuleBuilderOptions<T, string> ValidUserEmail() => rule
            .NotEmpty().WithMessage("The email address needs to be provided")
            .MaximumLength(60).WithMessage("The email cannot exceed 60 characters")
            .EmailAddress().WithMessage("Please provide a valid email address");

        public IRuleBuilderOptions<T, string> ValidUserPassword() => rule
            .NotEmpty().WithMessage("The password must be entered")
            .Matches(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[^\w\s]).{8,}$")
            .WithMessage("The password must be at least 8 characters long and contain letters, numbers, and special characters");
    }
}
