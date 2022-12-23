using System.Text.Json;

public class AccountView
{
    public Guid ID { get; set; } 

    public string Username { get; set; }

    public string Email { get; set; }

    public string Avatar { get; set; } = "";

    public PlayerState Status { get; set; } = PlayerState.Online;  

    public bool isAdmin { get; set; } = false;

    public AccountView() {
        this.ID = Guid.NewGuid();
        this.Username = "";
        this.Email = "";
    }

    public AccountView(Account account) {
        this.ID = account.ID;
        this.Username = account.Username;
        this.Email =  account.Email;
        this.Status = account.Status;
        this.isAdmin = account.isAdmin;
    }

    public string ToJsonString() {
        return JsonSerializer.Serialize<AccountView>(this);
    }
    
}
