public interface IAccountService {
    public Task<AccountView> GetAccountViewAsync(Guid uid);
}