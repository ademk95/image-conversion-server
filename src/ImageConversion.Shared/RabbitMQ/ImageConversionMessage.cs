namespace ImageConversion.Shared.RabbitMQ;

public class ImageConversionMessage
{
    public string Id { get; set; } = null!;
    public string TargetExtension { get; set; } = null!;
    public byte[] Content { get; set; } = null!;
    public string ConnId { get; set; }
}
