using System.Text.Json.Serialization;

public class AdAccountDto
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Ad? Ad { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AccountPoco? AccountPoco { get; set; }
    
    public bool Blocked { get; set; }

    public int Viewed { get; set; }   

    public int Clicked { get; set; }
}