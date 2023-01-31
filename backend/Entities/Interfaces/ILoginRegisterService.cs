public interface ILoginRegisterService
{
    //Account
    public Task<string> Login(string? authorization);
    public Task<Guid> Register(Account newAccount);

    //Guest
    public Task<string> LoginGuest(string username);

    //Logout
    public Task<string> Logout();
}
