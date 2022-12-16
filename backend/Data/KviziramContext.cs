
using backend.Data.Models;
using StackExchange.Redis;

public class KviziramContext
{
    public Account? caller { get; set; }
    public IDatabase Redis { get; set; }

    public KviziramContext(IDatabase redisInjection) {
        Redis = redisInjection;
    }
    
}
