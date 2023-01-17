using Microsoft.AspNetCore.SignalR;
using Neo4jClient;
using StackExchange.Redis;

public class GameHub: Hub
{
    KviziramContext _context;
    IDatabase _redis;
    IGraphClient _neo;
    Utility _util;
    ISubscriber _matchPubSub;

    public GameHub(KviziramContext context, Utility utility) {
        _context = context;
        _redis = context.Redis.GetDatabase();
        _neo = context.Neo;
        _util = utility;
        _matchPubSub = context.Redis.GetSubscriber();
    }

    public override Task OnConnectedAsync() {
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception) {
        return base.OnDisconnectedAsync(exception);
    }

    
}