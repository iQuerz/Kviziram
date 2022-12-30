public interface IAccountService 
{
    public Task<AccountPoco> GetAccountAsync(Guid? uid);

    public Task<List<AccountPoco>> GetFriendsAsync(RelationshipState rState);

    public Task<string> RequestRelationshipAsync(Guid fuid);
    public Task<string> AnswerRelationshipAsync(Guid fuID, RelationshipState answer);
    public Task<string> RemoveRelationshipAsync(Guid fuID);

    public Task<bool> RateQuizAsync(Guid quID, QuizRatingDto newRating);
    public Task<bool> RemoveRatingAsync(Guid quID);

}