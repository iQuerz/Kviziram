public interface ILoginRegisterService
{
    //Account
    public Task<string> Login(string? authorization);
    public Task<bool> Register(Account newAccount);

    //Guest
    public Task<string> LoginGuest(string username);
}
