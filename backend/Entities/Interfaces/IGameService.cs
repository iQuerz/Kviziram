public interface IGameService
{
    public Task<List<GameDto>?> GetPublicGamesAsync(FromToDate fromToDate, int skip, int limit, bool asc);
    public Task<Match?> CreateGameAsync(Match game); 
    public Task<string> StartGameAsync(string inviteCode);
    public Task<GameDto?> GetGameInformationAsync(string inviteCode);

    public Task<GameDto?> ConvertMatchToGameDtoAsync(Match? game);
    public Task<Match?> SaveGameToHistoryAsync(Match game);
    public Task<bool> RemoveGameFromRedisAsync(string inviteCode);
    
    public Task<Dictionary<string, AccountPoco>> GetGameLobbyAsync(string inviteCode);
    public Task<Dictionary<string, int>> GetGameScoresAsync(string inviteCode);
    public Task<List<string>> GetGameChatAsync(string inviteCode, int start = 0, int stop = 100);
    public Task<QuestionDto?> GetGameCurrentQuestionAsync(string inviteCode, Guid quizID);
    public Task<List<GameDto>?> GetLastPlayedGamesAsync(Guid playerGuid);
    public Task<Match?> GetGameAsync(string inviteCode);

    public Task SendInviteAsync(Guid auID, string inviteCode);
    public Task<List<GameInviteDto>?> GetAllInvitesAsync(Guid auID);
    public Task ClickInviteAsync(GameInviteDto invite);

    // public Task AddToLobby(string inviteCode, Guid auID, string sid);

}
