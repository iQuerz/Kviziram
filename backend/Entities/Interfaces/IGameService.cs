public interface IGameService
{
    public Task<Match?> CreateGameAsync(Match game); 
    
}
