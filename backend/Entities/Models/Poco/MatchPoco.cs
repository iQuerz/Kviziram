using System.Text.Json;

public class MatchPoco
{
    public Guid ID { get; set; } 

    public string? Created { get; set; }

    public string? InviteCode { get; set; }

    public GameState? GameState { get; set; }
    
    public Guid? HostID { get; set; }

    public Guid? WinnerID { get; set; }

    public string? Guests { get; set; }

    public MatchPoco(Match match) {
        this.ID = match.ID;
        this.Created = JsonSerializer.Serialize<DateTime?>(match.Created);
        this.GameState = match.GameState;
        this.InviteCode = match.InviteCode;
        this.HostID = match.HostID;
        this.WinnerID = match.WinnerID;
        this.Guests = (match.Guests == null) ? null : (JsonSerializer.Serialize<Dictionary<string, int>>(match.Guests));
    }

    
}
