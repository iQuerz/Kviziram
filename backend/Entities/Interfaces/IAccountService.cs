public interface IAccountService 
{
    public Task<AccountView> GetAccountViewAsync(Guid uid);
    public Task<string> RequestRelationshipAsync(Guid fuid);
    public Task<string> AnswerRelationshipAsync(Guid fuID, RelationshipState answer);
    public Task<string> RemoveRelationshipAsync(Guid fuID);
}