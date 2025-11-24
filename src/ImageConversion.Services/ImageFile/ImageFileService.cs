using ImageConversion.Data;
using ImageConversion.Services.Hubs;
using ImageConversion.Services.RabbitMQ.Publisher;
using ImageConversion.Shared.ImageFiles;
using ImageConversion.Shared.RabbitMQ;
using ImageMagick;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ImageConversion.Services.ImageFile;

public class ImageFileService : IImageFileService
{
    private readonly IRabbitMQService _rabbitMQService;
    private readonly ImageConversionContext _context;
    private readonly IHubContext<ImageConversionHub> _hub;

    public ImageFileService(
        IHubContext<ImageConversionHub> hub,
        IRabbitMQService rabbitMQService,
        ImageConversionContext context)
    {
        _rabbitMQService = rabbitMQService;
        _context = context;
        _hub = hub;
    }

    public async Task Conversion(IFormFile file, string target, string connId)
    {
        try
        {
            byte[] data;
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                data = ms.ToArray();
            }

            Data.Model.ImageFile imageFile = new()
            {
                FileName = file.FileName,
                SourceExtension = Path.GetExtension(file.FileName),
                TargetExtension = $".{target}",             
                ProcessStatus = Data.Enums.ProcessStatus.InProgress,
                Content = Array.Empty<byte>()
            };

            await _context.ImageFiles.AddAsync(imageFile);
            await _context.SaveChangesAsync();

            ImageConversionMessage message = new()
            {
                Id = imageFile.Id,
                Content = data.ToArray(),
                TargetExtension = imageFile.TargetExtension,
                ConnId = connId
            };
            await _rabbitMQService.SendImageFileToConversion(JsonSerializer.Serialize(message));
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task Convert(string id, byte[] content, string targetExtension, string connId)
    {
        try
        {
            using var image = new MagickImage(content);

            var format = targetExtension.ToLower() switch
            {
                ".webp" => MagickFormat.WebP,
                ".png" => MagickFormat.Png,
                ".jpg" or ".jpeg" => MagickFormat.Jpeg,
                _ => throw new NotSupportedException("Unsupported target")
            };

            image.Format = format;

            var result = image.ToByteArray();

            var imageFile = await _context.ImageFiles.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (imageFile == null)
            {
                throw new KeyNotFoundException($"Image with Id {id} not found");
            }

            imageFile.ProcessStatus = Data.Enums.ProcessStatus.Completed;
            imageFile.Content = result;
            imageFile.FinishedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _hub.Clients.Client(connId).SendAsync("ConversionCompleted", imageFile.Id);
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task<GetImageFileResponseDto> GetById(string id)
    {
        var imageFile = await _context.ImageFiles
            .Where(x => x.Id == id)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (imageFile == null || imageFile.Content == null)
        {
            throw new KeyNotFoundException();
        }

        return new()
        {
            Content = imageFile.Content,
            FileName = imageFile.FileName,
            TargetExtension = imageFile.TargetExtension
        };
    }

    public async Task<List<GetImageFileResponseDto>> Get()
    {
        var imageFiles = await _context.ImageFiles
            .AsNoTracking()
            .OrderByDescending(o => o.CreatedAt)
            .Select(s => new GetImageFileResponseDto
            {
                Content = s.Content,
                FileName = s.FileName,
                TargetExtension = s.TargetExtension
            })
            .ToListAsync();

        return imageFiles;
    }
}
