public interface IGameService
{
    public Task<List<GameDto>?> GetPublicGamesAsync(FromToDate fromToDate, int skip, int limit, bool asc);
    public Task<Match?> CreateGameAsync(Match game); 
    
}
