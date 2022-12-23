public interface IAccountService {
    public Task<AccountView> GetAccountViewAsync(Guid uid);
    public Task<RelationshipState> RequestRelationshipAsync(Guid fuid);
}