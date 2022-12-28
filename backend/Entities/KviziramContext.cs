
using StackExchange.Redis;
using Neo4jClient;

public class KviziramContext
{
    public string? SID { get; set; }
    public AccountPoco? AccountCaller { get; set; }
    public Guest? GuestCaller { get; set; }
    public IConnectionMultiplexer Redis { get; set; }
    public IGraphClient Neo { get; set; }


    public KviziramContext(IConnectionMultiplexer redisInjection, IGraphClient neoInjection) {
        Redis = redisInjection;
        Neo = neoInjection;
    }
    
}
