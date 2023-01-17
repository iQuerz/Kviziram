public interface IMatchService
{
    public Task<Match?> GetMatchAsync(Guid muID);
    public Task<string> SaveMatchAsync(Match match);
    // public Task<Match?> DeleteMatchAsync(string matchID);

    public Task<List<Match>?> SearchMatchesHistoryAsync(MatchQuery matchQuery, FromToDate? fromToDate, int skip, int limit);

}