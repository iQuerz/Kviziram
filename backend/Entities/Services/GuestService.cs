using StackExchange.Redis;
using Neo4jClient;

public class GuestService: IGuestService
{
    private IDatabase _redis;
    private IGraphClient _neo;
    
    public GuestService(KviziramContext context) {
        _redis = context.Redis.GetDatabase();
        _neo = context.Neo;
    }

    public async Task AddString(string key, string value) {
        await _redis.StringSetAsync(key, value);
    }

    public async Task<string?> GetString(string key) {
        return await _redis.StringGetAsync(key);
    }
    
}
