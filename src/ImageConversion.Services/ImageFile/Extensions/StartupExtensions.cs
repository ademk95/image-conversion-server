using Microsoft.Extensions.DependencyInjection;

namespace ImageConversion.Services.ImageFile.Extensions;

public static class StartupExtensions
{
    public static IServiceCollection AddImageFileServices(this IServiceCollection services)
    {
        services.AddScoped<IImageFileService, ImageFileService>();
        return services;
    }
}
