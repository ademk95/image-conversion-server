using ImageConversion.Shared.ImageFiles;
using Microsoft.AspNetCore.Http;

namespace ImageConversion.Services.ImageFile;

public interface IImageFileService
{
    Task Conversion(IFormFile file, string target, string connId);
    Task Convert(string id, byte[] content, string targetExtension, string connId);
    Task<GetImageFileResponseDto> GetById(string id);
    Task<List<GetImageFileResponseDto>> Get();
}
