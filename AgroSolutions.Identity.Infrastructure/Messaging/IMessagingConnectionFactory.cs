using RabbitMQ.Client;

namespace AgroSolutions.Identity.Infrastructure.Messaging;

public interface IMessagingConnectionFactory
{
    Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken);
}
