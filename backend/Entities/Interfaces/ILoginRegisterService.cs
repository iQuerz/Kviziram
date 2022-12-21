public interface ILoginRegisterService
{
    public Task<string> login(string? authorization);
    public Task<bool> register(Account newAccount);
}
