using RabbitMQ.Client;

namespace AgroSolutions.Identity.Infrastructure.Messaging;

public interface IRabbitConnectionProvider
{
    Task<IConnection> GetConnectionAsync();
}
