namespace ImageConversion.Shared.Constant;

public static class RabbitMqSettings
{
    public const string QueueName = "image_conversion_queue";
    public const string ExchangeName = "image_conversion_exchange";
    public const string ExchangeRouteKey = "image_conversion_route_key";
}

public static class RedisSettings
{
    public const string RedisImageConversionCacheKey = "image_conversions";
}
