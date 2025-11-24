namespace ImageConversion.Shared.ImageFiles;

public class GetImageFileResponseDto
{
    public string FileName { get; set; }
    public string TargetExtension { get; set; }
    public byte[] Content { get; set; }
}
