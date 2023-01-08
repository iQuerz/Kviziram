public class MatchDto
{
    public Guid ID { get; set; } 

    public bool IsSearchable { get; set; }

    public string Created { get; set; }

    public string InviteCode { get; set; }

    public GameState GameState { get; set; }
    
    public Guid HostID { get; set; }

    public Guid WinnerID { get; set; }

    public string Guests { get; set; }
}