using AgroSolutions.Identity.Application.Behaviors;
using AgroSolutions.Identity.Application.Notifications;
using AgroSolutions.Identity.Application.Queries.AuthenticateUser;
using AgroSolutions.Identity.Domain.Notifications;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AgroSolutions.Identity.Application;

public static class ApplicationModule
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication()
        {
            services
                .AddMediatR()
                .AddFluentValidation()
                .AddNotification();

            return services;
        }

        private IServiceCollection AddMediatR()
        {
            services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining<AuthenticateUserQuery>());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }

        private IServiceCollection AddFluentValidation()
        {
            services
                .AddFluentValidationAutoValidation(o => o.DisableDataAnnotationsValidation = true)
                .AddValidatorsFromAssemblyContaining<AuthenticateUserQueryValidator>();

            return services;
        }

        private IServiceCollection AddNotification()
        {
            services.AddScoped<INotificationContext, NotificationContext>();

            return services;
        }
    }
}
