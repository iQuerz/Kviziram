using Neo4jClient;
using StackExchange.Redis;

public class GameService: IGameService
{
    private IDatabase _redis;
    private IGraphClient _neo;
    
    public GameService(KviziramContext context) {
        _redis = context.Redis.GetDatabase();
        _neo = context.Neo;
    }
    
}
