using StackExchange.Redis;
using Neo4jClient;
using System.Text.Json;

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
    public async Task<Match?> GetMatchAsync(Guid muID) {
        return await GetMatchQueryAsync(muID);
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
    public async Task<Match?> GetMatchQueryAsync(Guid muID) {
        var query = _neo.Cypher
            .OptionalMatch("(a:Account)-[p:PARTICIPATED_IN]->(m:Match)-[:USED]->(q:Quiz)")
            .Where((MatchPoco m) => m.ID == muID)
            .Return((a,p,m,q) => new {
                Accounts = a.CollectAs<AccountPoco>(),
                AccountScores = p.CollectAs<ParticipatedInDto>(),
                MatchPoco = m.As<MatchDto>(),
                QuizID =  q.As<Quiz>().ID
            });
        Console.WriteLine(query.Query.DebugQueryText);
        var result = (await query.ResultsAsync).Single();

        if (result.MatchPoco == null)
            return null;
        Match match = new Match();
        match.ID = result.MatchPoco.ID;
        match.IsSearchable = result.MatchPoco.IsSearchable;
        if (result.MatchPoco.Created != null) match.Created = JsonSerializer.Deserialize<DateTime?>(result.MatchPoco.Created);
        match.InviteCode = result.MatchPoco.InviteCode;
        match.GameState = result.MatchPoco.GameState;
        match.HostID = result.MatchPoco.HostID;
        match.WinnerID = result.MatchPoco.WinnerID;
        if (result.MatchPoco.Guests != null) match.Guests = JsonSerializer.Deserialize<Dictionary<string, int>?>(result.MatchPoco.Guests);

        match.GetPlayerIDsScores = new List<ParticipatedInDto>();
        for (int i = 0; i < result.Accounts.Count(); i++) {
            result.AccountScores.ElementAt(i).Account = result.Accounts.ElementAt(i);
            match.GetPlayerIDsScores.Add(result.AccountScores.ElementAt(i));
        }
        Console.WriteLine(result.QuizID);
        match.QuizID = result.QuizID;

        return match;
    }

    public async Task SaveMatchQueryAsync(Match match) {        
        if (match.SetPlayerIDsScores != null) {
            MatchPoco matchPoco = new MatchPoco(match);
            var accountGuids = match.SetPlayerIDsScores.Keys.ToArray();
            var gameScores = match.SetPlayerIDsScores.Values.ToArray();

            var queryTest = _neo.Cypher
                .Match("(q:Quiz)")
                .Where((Quiz q) => q.ID == match.QuizID)
                .Merge("(m:Match $prop)")
                .WithParam("prop", matchPoco);
            Console.WriteLine(queryTest.Query.DebugQueryText);
            Console.WriteLine("-----------------------------");
            var query = _neo.Cypher
                .Match("(q:Quiz)")
                .Where((Quiz q) => q.ID == match.QuizID)
                .Merge("(m:Match { ID: $mID, IsSearchable: $mIsSearchable, Created: $mCreated, InviteCode: $mInviteCode, GameState: $mGameState, HostID: $mHostID , WinnerID: $mWinnerID, Guests: $mGuests})-[u:USED]->(q)")
                .WithParams( new { 
                    mID = matchPoco.ID,
                    mIsSearchable = matchPoco.IsSearchable,
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
            Console.WriteLine(query.Query.DebugQueryText);
            await query.ExecuteWithoutResultsAsync();
        }
    }
    #endregion
}