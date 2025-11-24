using ImageConversion.Services.RabbitMQ.Consumer;
using ImageConversion.Services.RabbitMQ.Publisher;
using Microsoft.Extensions.DependencyInjection;

namespace ImageConversion.Services.RabbitMQ.Extensions;

public static class StartupExtensions
{
    public static IServiceCollection AddRabbitMQServices(this IServiceCollection services)
    {
        services.AddSingleton<IRabbitMQService, RabbitMQService>();
        services.AddHostedService<RabbitMQImageFileToConversionConsumerService>();

        return services;
    }
}
