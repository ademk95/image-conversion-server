using StackExchange.Redis;

namespace ImageConversion.Services.Redis;

public class RedisService : IRedisService
{
    private readonly IDatabase _db;

    public RedisService(IConnectionMultiplexer mux)
    {
        _db = mux.GetDatabase();
    }

    public async Task<bool> SetAsync(string key, string value)
    {
        return await _db.StringSetAsync(key, value);
    }

    public async Task<string?> GetAsync(string key)
    {
        var v = await _db.StringGetAsync(key);
        return v.HasValue ? v.ToString() : null;
    }
}
