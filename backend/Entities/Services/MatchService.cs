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
    public async Task SaveMatchQueryAsync(Match match) {
        MatchPoco matchPoco = new MatchPoco(match);
        
        if (match.PlayerIDsScores != null) {
            var accountGuids = match.PlayerIDsScores.Keys.ToArray();
            var gameScores = match.PlayerIDsScores.Values.ToArray();
            var query = _neo.Cypher
                .Match("(q:Quiz)")
                .Where((Quiz q) => q.ID == match.QuizID)
                .Merge("(m:Match { ID: $mID, Created: $mCreated, InviteCode: $mInviteCode, GameState: $mGameState, HostID: $mHostID , WinnerID: $mWinnerID, Guests: $mGuests})-[u:USED]->(q)")
                .WithParams( new { 
                    mID = matchPoco.ID,
                    mCreated = matchPoco.Created,
                    mInviteCode = matchPoco.InviteCode,
                    mGameState = matchPoco.GameState,
                    mHostID = matchPoco.HostID,
                    mWinnerID = matchPoco.WinnerID,
                    mGuests = matchPoco.Guests 
                    })
                .With("m, $propac AS accounts, $propscore AS scores")
                .WithParams( new { propac = accountGuids, propscore = gameScores})
                .Unwind("range(0,size(accounts)-1)", "index")
                .OptionalMatch("(a:Account)")
                .Where("a.ID = accounts[index]")
                .Merge("(a)-[p:PARTICIPATED_IN]->(m)")
                .Set("p.GameScore = scores[index]");
            await query.ExecuteWithoutResultsAsync();
        }
    }
    #endregion
}