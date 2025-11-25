using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace ImageConversion.Services.Redis.Extensions;

public static class StartupExtensions
{
    public static IServiceCollection AddRedisServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(_ =>
           ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")));

        services.AddSingleton<IRedisService, RedisService>();
        return services;
    }
}
