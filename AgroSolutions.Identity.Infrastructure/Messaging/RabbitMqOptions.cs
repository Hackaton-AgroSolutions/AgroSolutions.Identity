namespace AgroSolutions.Identity.Infrastructure.Messaging;

public class RabbitMqOptions
{
    public string Host { get; set; } = default!;
    public int Port { get; set; }
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Exchange { get; set; } = default!;
    public IEnumerable<RabbitMqDestination> Destinations { get; set; } = [];
}

public class RabbitMqDestination
{
    public string Id { get; set; } = default!;
    public string Queue { get; set; } = default!;
    public string RoutingKey { get; set; } = default!;
}
