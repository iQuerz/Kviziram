using System.Text.Json.Serialization;

public class AdCategoryDto
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Ad? Ad { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Category? Category { get; set; }
    
    public int Paid { get; set; }
}