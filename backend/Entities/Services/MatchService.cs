using StackExchange.Redis;
using Neo4jClient;

public class MatchService: IMatchService
{
    private KviziramContext _context;
    private IDatabase _redis;
    private IGraphClient _neo;
    private Utility _util;
    
    public MatchService(KviziramContext context, Utility utility) {
        _context = context;
        _redis = context.Redis.GetDatabase();
        _neo = context.Neo;
        _util = utility;
    }

    #region Main Functions
    public Task<Match?> GetMatchAsync(Match match) {
        return null;
    }

    public Task<Match?> CreateMatchAsync(Match match) {
        return null;
    }

    public async Task<string> SaveMatchAsync(Match match) {
        await SaveMatchQueryAsync(match);
        return Msg.SavedMatch;
    }

    public Task<Match?> PutMatchAsync(Match match) {
        return null;
    }
    public Task<Match?> DeleteMatchAsync(string matchID) {
        return null;
    }
    #endregion

    #region Helper Functions
    public async Task<string> SaveMatchQueryAsync(Match match) {
        MatchPoco matchPoco = new MatchPoco(match);
        
        List<Guid> accountGuids;
        List<int> gameScores;

        if (match.PlayerIDsScores != null) {
            accountGuids = match.PlayerIDsScores.Keys.ToList();
            gameScores = match.PlayerIDsScores.Values.ToList();

            var query = _neo.Cypher
            .Match("(q:Quiz)")
            .Where((Quiz q) => q.ID == match.QuizID)
            .Merge("(m:Match $prop)-[u:USED]->(q)")
            .WithParams( new { prop = matchPoco})
            .With("m")
            .Unwind(accountGuids, "accountID")
            .Unwind(gameScores, "score")
            .OptionalMatch("(a:Account)")
            .Where("a.ID = accountID")
            .Merge("(a)-[p:PARTICIPATED_IN]->(m)")
            .Set("r.GameScore = score");

            Console.WriteLine(query.Query.DebugQueryText);
        }
        return "";
    }
    #endregion
}