using AgroSolutions.Identity.Application;
using AgroSolutions.Identity.Domain.Notifications;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace AgroSolutions.Identity.Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators, INotificationContext notification) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;
    private readonly INotificationContext _notification = notification;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next(cancellationToken);

        ValidationContext<TRequest> context = new(request);

        IEnumerable<ValidationFailure> failures = [.. _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)];

        if (failures.Any())
        {
            foreach (ValidationFailure error in failures)
            {
                _notification.AddValidation(
                    char.ToLowerInvariant(error.PropertyName[0]) + error.PropertyName[1..],
                    error.ErrorMessage
                );
            }

            return default!;
        }

        return await next(cancellationToken);
    }
}
