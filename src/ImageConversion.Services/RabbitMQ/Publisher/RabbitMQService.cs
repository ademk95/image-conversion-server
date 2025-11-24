using ImageConversion.Shared.Constant;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;

namespace ImageConversion.Services.RabbitMQ.Publisher;

public class RabbitMQService : IRabbitMQService
{
    private readonly IConfiguration _config;
    public RabbitMQService(IConfiguration config)
    {
        _config = config;
    }

    private ConnectionFactory GetConnection()
    {
        return new ConnectionFactory()
        {
            HostName = _config["RabbitMQ:HostName"],
            Port = int.Parse(_config["RabbitMQ:Port"]),
            UserName = _config["RabbitMQ:UserName"],
            Password = _config["RabbitMQ:Password"],
            VirtualHost = _config["RabbitMQ:VirtualHost"]
        };
    }

    public async Task SendImageFileToConversion(string message)
    {
        var factory = GetConnection();

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: RabbitMqSettings.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        await channel.ExchangeDeclareAsync(RabbitMqSettings.ExchangeName, ExchangeType.Direct, durable: true, autoDelete: false);
        await channel.QueueBindAsync(RabbitMqSettings.QueueName, RabbitMqSettings.ExchangeName, routingKey: RabbitMqSettings.ExchangeRouteKey);

        var body = Encoding.UTF8.GetBytes(message);

        var properties = new BasicProperties
        {
            Persistent = true,
            Headers = new Dictionary<string, object>
            {
                { "tryCount", 1 }
            }
        };

        await channel.BasicPublishAsync(exchange: RabbitMqSettings.ExchangeName, routingKey: RabbitMqSettings.ExchangeRouteKey, true, body: body, basicProperties: properties);
    }
}
