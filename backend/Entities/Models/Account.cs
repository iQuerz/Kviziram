using System.ComponentModel.DataAnnotations;

public class Account
{
    public Guid ID { get; set; } 

    [Required]
    public string Username { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    public string Avatar { get; set; } = "";

    public PlayerState Status { get; set; } = PlayerState.Online;  

    public bool isAdmin { get; set; } = false;

    public Account() {
        this.Username = "";
        this.Email = "";
        this.Password = "";
    }
}
