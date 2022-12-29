using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

public class Quiz
{
    public Guid ID { get; set; } 

    [Required]
    public string? Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Guid? CreatorID { get; set; } = null;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Guid? AchievementID { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Guid? QuestionsID { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<QuestionDto>? Questions { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Category? Category { get; set; }

    public Quiz() {
        this.ID = Guid.NewGuid();
        this.Name = "";
    }

    public string QuestionsToJsonString() {
        if (Questions == null) return "";
        return JsonSerializer.Serialize<List<QuestionDto>>(this.Questions);

    }
}