using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Achievement
{
    public Guid ID { get; set; } 

    [Required]
    public string? Name { get; set; }

    [Required]
    public string? Description { get; set; }

    public string Picture { get; set; } = "";

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Progress { get; set; }
}