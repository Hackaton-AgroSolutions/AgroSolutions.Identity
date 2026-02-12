using RabbitMQ.Client;

namespace AgroSolutions.Identity.Infrastructure.Messaging;

public static class RabbitMqConnection
{
    public static async Task InitializeAsync(IChannel channel, RabbitMqOptions options)
    {
        await channel.ExchangeDeclareAsync(
            exchange: options.Exchange,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false);

        foreach (RabbitMqDestination destination in options.Destinations)
        {
            await channel.QueueDeclareAsync(
                queue: destination.Queue,
                durable: true,
                exclusive: false,
                autoDelete: false);
            await channel.QueueBindAsync(
                queue: destination.Queue,
                exchange: options.Exchange,
                routingKey: destination.RoutingKey);
        }
    }
}
