using System.Text.Json;

public class Guest
{
    public Guid ID { get; set; } 

    public string Username { get; set; }

    public Guest() {
        this.Username = "";
    }

    public string ToJsonString() {
        return JsonSerializer.Serialize<Guest>(this);
    }
    
}