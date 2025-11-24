using ImageConversion.Services.ImageFile;
using ImageConversion.Shared.Constant;
using ImageConversion.Shared.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace ImageConversion.Services.RabbitMQ.Consumer;

internal class RabbitMQImageFileToConversionConsumerService : IHostedService
{
    private readonly int _channelCount = 1;

    private readonly IConfiguration _config;
    private IConnection _connection;
    private IChannel[] _channels = [];
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public RabbitMQImageFileToConversionConsumerService(IConfiguration config, IServiceScopeFactory serviceScopeFactory)
    {
        _config = config;
        _channels = new IChannel[_channelCount];
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var factory = new ConnectionFactory()
            {
                HostName = _config["RabbitMQ:HostName"],
                Port = int.Parse(_config["RabbitMQ:Port"]),
                UserName = _config["RabbitMQ:UserName"],
                Password = _config["RabbitMQ:Password"],
                VirtualHost = _config["RabbitMQ:VirtualHost"],
                RequestedHeartbeat = TimeSpan.FromMinutes(10)
            };

            _connection = await factory.CreateConnectionAsync(cancellationToken);

            for (int channelIndex = 0; channelIndex < _channelCount; channelIndex++)
            {
                int currentIndex = channelIndex;

                _channels[currentIndex] = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
                await _channels[currentIndex].ExchangeDeclareAsync(RabbitMqSettings.ExchangeName, ExchangeType.Direct, true, false);
                await _channels[currentIndex].QueueDeclareAsync(queue: RabbitMqSettings.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null, cancellationToken: cancellationToken);
                await _channels[currentIndex].QueueBindAsync(RabbitMqSettings.QueueName, RabbitMqSettings.ExchangeName, RabbitMqSettings.ExchangeRouteKey);
                await _channels[currentIndex].BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: cancellationToken);

                var consumer = new AsyncEventingBasicConsumer(_channels[currentIndex]);

                consumer.ReceivedAsync += async (model, ea) =>
                {
                    try
                    {
                        byte[] body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);

                        var messageJson = JsonSerializer.Deserialize<ImageConversionMessage>(message);

                        using (var scope = _serviceScopeFactory.CreateScope())
                        {
                            var service = scope.ServiceProvider.GetRequiredService<IImageFileService>();
                            await Task.Delay(2000); // Simulate processing time
                            await service.Convert(messageJson.Id, messageJson.Content, messageJson.TargetExtension, messageJson.ConnId);
                        }

                        await _channels[currentIndex].BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                    catch (System.Exception ex)
                    {
                        await _channels[currentIndex].BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
                    }
                };

                await _channels[currentIndex].BasicConsumeAsync(RabbitMqSettings.QueueName, autoAck: false, consumerTag: $"image-conversion-{currentIndex + 1}", consumer: consumer, cancellationToken: cancellationToken);
            }

        }
        catch (Exception)
        {

            throw;
        }

        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var channel in _channels)
        {
            if (channel != null)
            {
                await Task.Run(() =>
                {
                    channel.DisposeAsync();
                    channel.CloseAsync(cancellationToken);
                });
            }
        }

        if (_connection != null)
        {
            await Task.Run(() =>
            {
                _connection.DisposeAsync();
                _connection.CloseAsync(cancellationToken);
            });
        }
    }
}
