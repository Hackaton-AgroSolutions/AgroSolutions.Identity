using AgroSolutions.Identity.Domain.Common;
using AgroSolutions.Identity.Domain.Events;
using AgroSolutions.Identity.Domain.Messaging;
using AgroSolutions.Identity.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using RabbitMQ.Client;
using Serilog;
using System.Text;
using System.Text.Json;
using static AgroSolutions.Identity.Infrastructure.Messaging.RabbitMqOptions;

namespace AgroSolutions.Identity.Infrastructure.Messaging;

public class RabbitMqPublisher(IMessagingConnectionFactory factory, IOptions<RabbitMqOptions> options, IHttpContextAccessor httpContextAccessor) : IEventPublisher
{
    private readonly IMessagingConnectionFactory _factory = factory;
    private readonly RabbitMqOptions _rabbitMqOptions = options.Value;

    public async Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken, string? correlationId = default)
    {
        IChannel channel = await _factory.CreateChannelAsync();
        if (httpContextAccessor.HttpContext is not null)
        {
            httpContextAccessor.HttpContext!.Response.Headers.TryGetValue("X-Correlation-ID", out StringValues stringValues);
            correlationId = stringValues.ToString();
        }
        BasicProperties basicProperties = new() { CorrelationId = correlationId };
        RabbitMqDestination rabbitMqDestination;

        switch (domainEvent)
        {
            case DeletedUserEvent deletedUser:
                rabbitMqDestination = _rabbitMqOptions.Destinations.First(d => d.Id.Equals(EventType.DeletedUser.GetDescription(), StringComparison.OrdinalIgnoreCase));
                Log.Information("Adding the user deletion event with ID {UserId} to the RabbitMQ queue {Queue}.", deletedUser.UserId, rabbitMqDestination.Queue);
                byte[] body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(deletedUser));
                await channel.BasicPublishAsync(_rabbitMqOptions.Exchange, rabbitMqDestination.RoutingKey, false, basicProperties, body, cancellationToken);
                break;
        }
    }
}
