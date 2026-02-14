using AgroSolutions.Identity.Domain.Messaging;
using AgroSolutions.Identity.Domain.Repositories;
using AgroSolutions.Identity.Domain.Service;
using AgroSolutions.Identity.Infrastructure.Messaging;
using AgroSolutions.Identity.Infrastructure.Persistence;
using AgroSolutions.Identity.Infrastructure.Persistence.Repositories;
using AgroSolutions.Identity.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AgroSolutions.Identity.Infrastructure;

public static class InfrastructureModule
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(IConfiguration configuration)
        {
            services
                .AddMessageBroker(configuration)
                .AddAuthentication(configuration)
                .AddPersistence(configuration)
                .AddRepositories()
                .AddUnitOfWork();

            return services;
        }

        private IServiceCollection AddMessageBroker(IConfiguration configuration)
        {
            services.Configure<RabbitMqOptions>(configuration.GetSection("Messaging"));

            services.AddHttpContextAccessor();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IRabbitConnectionProvider, RabbitConnectionProvider>();
            services.AddScoped<IMessagingConnectionFactory, RabbitChannelFactory>();
            services.AddScoped<IEventPublisher, RabbitMqPublisher>();

            return services;
        }

        private IServiceCollection AddAuthentication(IConfiguration configuration)
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!))
                    };
                });

            services.AddScoped<IAuthService, AuthService>();

            return services;
        }

        private IServiceCollection AddPersistence(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection")!;
            services.AddDbContext<AgroSolutionsIdentityDbContext>(options => options.UseSqlServer(connectionString));

            return services;
        }

        private IServiceCollection AddRepositories()
        {
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }

        private IServiceCollection AddUnitOfWork()
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
