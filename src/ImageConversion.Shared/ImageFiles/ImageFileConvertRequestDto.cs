using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ImageConversion.Shared.ImageFiles;

public class ImageFileConvertRequestDto
{
    public IFormFile File { get; set; }
    
    public string Target { get; set; }
    public string ConnId { get; set; }
}
