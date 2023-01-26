public interface IAccountService 
{
    public Task<AccountPoco> GetAccountAsync(Guid? uid);
    public Task<List<AccountPoco>> GetAllAccountsAsync();

    public Task<List<AccountPoco>> GetFriendsAsync(RelationshipState rState);

    public Task<string> RequestRelationshipAsync(Guid fuid);
    public Task<string> AnswerRelationshipAsync(Guid fuID, RelationshipState answer);
    public Task<string> RemoveRelationshipAsync(Guid fuID);
    public Task<string> BlockRelationshipAsync(Guid fuID);

    public Task<QuizRatingDto?> GetRatingAsync(Guid quID);
    public Task<bool> AddRatingAsync(Guid quID, QuizRatingDto newRating);
    public Task<bool> UpdateRatingAsync(Guid quID, QuizRatingDto updatedRating);    
    public Task<bool> RemoveRatingAsync(Guid quID);

    public Task<List<Category>?> GetPreferredCategoriesAsync();
    public Task<string> SetPreferredCategoryAsync (List<Guid> categoryGuids);
    public Task<string> RemovePreferredCategoryAsync(Guid cuID);

    public Task<string> SetUpdateAchievementAsync(Guid auID, Guid acuID);
    public Task<List<Achievement>?> GetAccountAchievementsAsync(Guid auID);

    public Task<Ad?> GetRecommendedAdsAsync();
    public Task<List<AccountPoco>?> GetRecommendedFriendsAsync(Guid auID);
    public Task<List<AccountPoco>?> GetFriendsOfFriendsAsync(Guid auID);
    public Task<List<AccountPoco>?> RecommendedPlayersFromMatchAsync(Guid auID);
    public Task<List<Quiz>?> GetRecommendedQuizzesAsync();

    public Task<bool> AccountExistsAsync(Guid uID);

    public Task<AccountPoco?> GetMyAccount();
    public Task<List<AccountPoco>?> GetAccountsByUsername(string username);

}