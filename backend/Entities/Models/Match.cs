using System.Text.Json.Serialization;

public class Match
{
    public Guid ID { get; set; }
    
    public Guid HostID { get; set; }

    public Guid WinnerID { get; set; }

    public Guid QuizID { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<ParticipatedInDto>? Participated { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Guest>? GuestNames { get; set; }
    
}
