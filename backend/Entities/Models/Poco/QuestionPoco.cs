using System.Text.Json;

public class QuestionPoco
{
    public Guid ID { get; set; }

    public string? Info { get; set; }

    public List<QuestionDto>? DeserializeInfo() {
        if (this.Info != null)
            return JsonSerializer.Deserialize<List<QuestionDto>>(this.Info);
        return null;
    }

    
}