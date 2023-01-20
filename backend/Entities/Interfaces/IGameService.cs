public interface IGameService
{
    public Task<List<GameDto>?> GetPublicGamesAsync(FromToDate fromToDate, int skip, int limit, bool asc);
    public Task<Match?> CreateGameAsync(Match game); 
    public Task<string> StartGameAsync(string inviteCode);
    public Task<GameDto?> GetGameInformationAsync(string inviteCode);

    public Task<GameDto?> ConvertMatchToGameDtoAsync(Match? game);
    public Task<Match?> SaveGameToHistoryAsync(Match game);
    public Task<bool> RemoveGameFromRedisAsync(string inviteCode);
    
    public Task<string> GetGameLobbyAsync(string inviteCode);
    public Task<string> GetGameScoresAsync(string inviteCode);
    public Task<string> GetGameChatAsync(string inviteCode, int start = 0, int stop = 100);
    public Task<QuestionDto?> GetGameCurrentQuestionAsync(string inviteCode, Guid quizID);
    public Task<List<GameDto>?> GetLastPlayedGamesAsync(Guid playerGuid);

}
