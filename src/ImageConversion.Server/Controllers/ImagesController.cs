using ImageConversion.Services.ImageFile;
using ImageConversion.Shared.ImageFiles;
using Microsoft.AspNetCore.Mvc;

namespace ImageConversion.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageFileService _imageFileService;
        public ImagesController(IImageFileService imageFileService)
        {
            _imageFileService = imageFileService;
        }

        [HttpPost("convert")]
        public async Task<IActionResult> Convert([FromForm] ImageFileConvertRequestDto dto)
        {
            await _imageFileService.Conversion(dto.File, dto.Target, dto.ConnId);
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            return Ok(await _imageFileService.GetById(id));
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _imageFileService.Get());
        }
    }
}
