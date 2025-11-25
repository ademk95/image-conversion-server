namespace ImageConversion.Services.Redis;

public interface IRedisService
{
    Task<bool> SetAsync(string key, string value);
    Task<string?> GetAsync(string key);
}
