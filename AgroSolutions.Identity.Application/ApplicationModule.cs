using AgroSolutions.Identity.Application.Behaviors;
using AgroSolutions.Identity.Application.Notifications;
using AgroSolutions.Identity.Application.Queries.GetUserByEmailAndPassword;
using AgroSolutions.Identity.Domain.Notifications;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AgroSolutions.Identity.Application;

public static class ApplicationModule
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services
            .AddMediatR()
            .AddFluentValidation()
            .AddNotification();

        return services;
    }

    private static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining<GetUserByEmailAndPasswordQuery>());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }

    private static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services
            .AddFluentValidationAutoValidation(o => o.DisableDataAnnotationsValidation = true)
            .AddValidatorsFromAssemblyContaining<GetUserByEmailAndPasswordQueryValidator>();

        return services;
    }

    private static IServiceCollection AddNotification(this IServiceCollection services)
    {
        services.AddScoped<INotificationContext, NotificationContext>();

        return services;
    }
}
