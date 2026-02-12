using RabbitMQ.Client;

namespace AgroSolutions.Identity.Infrastructure.Messaging;

public class RabbitChannelFactory(IRabbitConnectionProvider provider) : IMessagingConnectionFactory
{
    private readonly IRabbitConnectionProvider _provider = provider;

    public async Task<IChannel> CreateChannelAsync()
    {
        IConnection connection = await _provider.GetConnectionAsync();
        return await connection.CreateChannelAsync();
    }
}
