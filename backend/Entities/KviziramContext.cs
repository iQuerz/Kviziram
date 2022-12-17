
using StackExchange.Redis;
using Neo4jClient;

public class KviziramContext
{
    public Account? Caller { get; set; }
    public IConnectionMultiplexer Redis { get; set; }
    public IGraphClient Neo { get; set; }


    public KviziramContext(IConnectionMultiplexer redisInjection, IGraphClient neoInjection) {
        Redis = redisInjection;
        Neo = neoInjection;
    }
    
}
