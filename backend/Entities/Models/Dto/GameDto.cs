using System.Text.Json.Serialization;

public class GameDto
{
    public DateTime Created { get; set; }

    public string InviteCode { get; set; } = string.Empty;
 
    public string HostName { get; set; } = string.Empty;

    public string QuizName { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public string TrophyName { get; set; } = string.Empty;

}