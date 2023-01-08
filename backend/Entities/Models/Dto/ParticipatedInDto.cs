using System.Text.Json.Serialization;

public class ParticipatedInDto
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AccountPoco? Account { get; set; }
    
    public int GameScore { get; set; }
}
