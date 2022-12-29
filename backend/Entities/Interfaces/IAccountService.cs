public interface IAccountService 
{
    public Task<AccountPoco> GetAccountAsync(Guid? uid);
    public Task<List<AccountPoco>> GetFriendsAsync(RelationshipState rState);
    public Task<string> RequestRelationshipAsync(Guid fuid);
    public Task<string> AnswerRelationshipAsync(Guid fuID, RelationshipState answer);
    public Task<string> RemoveRelationshipAsync(Guid fuID);
}