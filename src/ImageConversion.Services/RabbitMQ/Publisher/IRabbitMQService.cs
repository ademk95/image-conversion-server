namespace ImageConversion.Services.RabbitMQ.Publisher;

public interface IRabbitMQService
{
    Task SendImageFileToConversion(string message);
}
