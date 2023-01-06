public interface IMatchService
{
    public Task<Match?> GetMatchAsync(Match match);
    public Task<Match?> CreateMatchAsync(Match match);
    public Task<string> SaveMatchAsync(Match match);
    public Task<Match?> PutMatchAsync(Match match);
    public Task<Match?> DeleteMatchAsync(string matchID);

}