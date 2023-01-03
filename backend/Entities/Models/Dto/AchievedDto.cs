using System.Text.Json.Serialization;

public class AchievedDto
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AccountPoco? AccountPoco { get; set; }
    
    public int Progress { get; set; }
}
