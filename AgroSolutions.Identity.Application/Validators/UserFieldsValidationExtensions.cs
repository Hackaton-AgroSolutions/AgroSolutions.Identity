using FluentValidation;

namespace AgroSolutions.Identity.Application.Validators;

public static class UserFieldsValidationExtensions
{
    public const string MESSAGE_INVALID_USERID = "The user identifier is invalid";
    public const string MESSAGE_EMPTY_USERNAME = "The name needs to be provided";
    public const string MESSAGE_INVALID_LENGTH_USERNAME = "The name cannot exceed 60 characters";
    public const string MESSAGE_EMPTY_USEREMAIL = "The email address needs to be provided";
    public const string MESSAGE_INVALID_LENGTH_USEREMAIL = "The email cannot exceed 60 characters";
    public const string MESSAGE_INVALID_USEREMAIL = "Please provide a valid email address";
    public const string MESSAGE_EMPTY_USERPASSWORD = "The password must be entered";
    public const string MESSAGE_INVALID_USERPASSWORD = "The password must be at least 8 characters long and contain letters, numbers, and special characters";

    extension<T>(IRuleBuilder<T, int> rule)
    {
        public IRuleBuilderOptions<T, int> ValidUserId() => rule
            .GreaterThan(0).WithMessage(MESSAGE_INVALID_USERID);
    }

    extension<T>(IRuleBuilder<T, string> rule)
    {
        public IRuleBuilderOptions<T, string> ValidUserName() => rule
            .NotEmpty().WithMessage(MESSAGE_EMPTY_USERNAME)
            .MaximumLength(60).WithMessage(MESSAGE_INVALID_LENGTH_USERNAME);

        public IRuleBuilderOptions<T, string> ValidUserEmail() => rule
            .NotEmpty().WithMessage(MESSAGE_EMPTY_USEREMAIL)
            .MaximumLength(60).WithMessage(MESSAGE_INVALID_LENGTH_USEREMAIL)
            .EmailAddress().WithMessage(MESSAGE_INVALID_USEREMAIL);

        public IRuleBuilderOptions<T, string> ValidUserPassword() => rule
            .NotEmpty().WithMessage(MESSAGE_EMPTY_USERPASSWORD)
            .Matches(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[^\w\s]).{8,}$")
            .WithMessage(MESSAGE_INVALID_USERPASSWORD);
    }
}
