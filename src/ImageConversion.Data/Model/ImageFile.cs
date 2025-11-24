using ImageConversion.Data.Enums;

namespace ImageConversion.Data.Model;

public class ImageFile
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FileName { get; set; } = null!;
    public byte[] Content { get; set; } = null!;
    public string SourceExtension { get; set; } = null!;
    public string TargetExtension { get; set; } = null!;
    public int ProcessTime { get; set; } = 0;
    public ProcessStatus ProcessStatus { get; set; } = ProcessStatus.Unknown;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? FinishedAt { get; set; }
}
