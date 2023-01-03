using StackExchange.Redis;
using Neo4jClient;

public class AchievementService: IAchievementService
{
    private KviziramContext _context;
    private IDatabase _redis;
    private IGraphClient _neo;
    private Utility _util;
    
    public AchievementService(KviziramContext context, Utility utility) {
        _context = context;
        _redis = context.Redis.GetDatabase();
        _neo = context.Neo;
        _util = utility;
    }

    #region Main Functions
    public async Task<Achievement> GetAchievementAsync(Guid? uID) {
        Achievement? achievement = await GetAchievementQueryAsync(uID);
        if (achievement == null)
            throw new KviziramException(Msg.NoAchievement);
        return achievement;
    }

    public async Task<List<Achievement>> GetAllAchievementsAsync() {
        return await GetAllAchievementsQueryAsync();
    }

    public async Task<Achievement> CreateAchievementAsync(Achievement newAchievement) {
        newAchievement.ID = Guid.NewGuid();
        newAchievement.Progress = null;
        await CreateAchievementQueryAsync(newAchievement);
        return newAchievement;
    }

    public async Task<Achievement> UpdateAchievementAsync(Achievement updatedAchievement) {
        Achievement achievementExists = await GetAchievementAsync(updatedAchievement.ID);
        if (achievementExists == null)
            throw new KviziramException(Msg.NoAchievement);
        updatedAchievement.Progress = null;
        await UpdateAchievementQueryAsync(updatedAchievement);
        return updatedAchievement;
    }

    public async Task<string> DeleteAchievementAsync(Guid uID) {
        Achievement AchievementExists = await GetAchievementAsync(uID);
        if (AchievementExists == null)
            throw new KviziramException(Msg.NoAchievement);
        await DeleteAchievementQueryAsync(uID);
        return ("Achievement: " + uID + Msg.Deleted);
    }

    public async Task<List<AchievedDto>?> GetAchievementScoreboardAsync(Guid acuID) {
        Achievement? categoryExists = await GetAchievementAsync(acuID);
        if (categoryExists == null)
            throw new KviziramException(Msg.NoAchievement);
        return await GetAchievementScoreboardQueryAsync(acuID);
    }
    #endregion

    #region Helper Functions
    public async Task<Achievement?> GetAchievementQueryAsync(Guid? uID) {
        var query = await _neo.Cypher
            .OptionalMatch("(a:Achievement)")
            .Where((Achievement a) => a.ID == uID)
            .Return(a => a.As<Achievement>())
            .ResultsAsync;
        return query.Single();
    }

    public async Task<List<Achievement>> GetAllAchievementsQueryAsync() {
        var query = await _neo.Cypher
            .Match("(a:Achievement)")
            .Return(a => a.As<Achievement>())
            .ResultsAsync;
        return query.ToList();
    }

    public async Task CreateAchievementQueryAsync(Achievement newAchievement) {
        await _neo.Cypher
            .Create("(a:Achievement $prop)")
            .WithParam("prop", newAchievement)
            .ExecuteWithoutResultsAsync();
    }

    public async Task UpdateAchievementQueryAsync(Achievement updatedAchievement) {
        await _neo.Cypher
            .Match("(a:Achievement)")
            .Where((Achievement a) => a.ID == updatedAchievement.ID)
            .Set("a = $prop")
            .WithParam("prop", updatedAchievement)
            .ExecuteWithoutResultsAsync();
    }

    public async Task DeleteAchievementQueryAsync(Guid uID) {
        await _neo.Cypher
            .Match("(a:Achievement)")
            .Where((Achievement a) => a.ID == uID)
            .DetachDelete("a")
            .ExecuteWithoutResultsAsync();
    }

    public async Task<List<AchievedDto>?> GetAchievementScoreboardQueryAsync(Guid acuID) {
        var query = _neo.Cypher
            .OptionalMatch("(ac:Achievement)<-[r:ACHIEVED]-(a:Account)")
            .Where((Achievement ac) => ac.ID == acuID)
            .Return((a, r) => new {
                Accounts = a.CollectAs<AccountPoco>(),
                AchievementProgress = r.CollectAs<AchievedDto>()
            });
        Console.WriteLine(query.Query.DebugQueryText);
        var result = (await query.ResultsAsync).Single();
        if (result.AchievementProgress.Count() != 0) {
            for(int i = 0; i < result.AchievementProgress.Count(); i++)
                result.AchievementProgress.ElementAt(i).AccountPoco = result.Accounts.ElementAt(i);
            return result.AchievementProgress.OrderByDescending(p => p.Progress).ToList();
        }

        return null;
    }
    #endregion   
    
}