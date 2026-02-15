using RabbitMQ.Client;

namespace AgroSolutions.Identity.Infrastructure.Messaging;

public class RabbitChannelFactory(IRabbitConnectionProvider provider) : IMessagingConnectionFactory
{
    private readonly IRabbitConnectionProvider _provider = provider;

    public async Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken)
    {
        IConnection connection = await _provider.GetConnectionAsync(cancellationToken);
        return await connection.CreateChannelAsync(cancellationToken: cancellationToken);
    }
}
