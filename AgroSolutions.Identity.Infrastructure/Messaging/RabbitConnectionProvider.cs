using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace AgroSolutions.Identity.Infrastructure.Messaging;

public class RabbitConnectionProvider : IRabbitConnectionProvider
{
    private readonly ConnectionFactory _factory;
    private IConnection? _connection;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public RabbitConnectionProvider(IOptions<RabbitMqOptions> options)
    {
        _factory = new ConnectionFactory
        {
            HostName = options.Value.Host,
            Port = options.Value.Port,
            UserName = options.Value.Username,
            Password = options.Value.Password,
            AutomaticRecoveryEnabled = true
        };
    }

    public async Task<IConnection> GetConnectionAsync()
    {
        if (_connection is not null && _connection.IsOpen)
            return _connection;

        await _lock.WaitAsync();
        try
        {
            if (_connection is null || !_connection.IsOpen)
                _connection = await _factory.CreateConnectionAsync();

            return _connection;
        }
        finally
        {
            _lock.Release();
        }
    }
}
