using System.Text.Json;

public class QuizPoco
{
    public Guid ID { get; set; } 

    public string? Name { get; set; }

    public QuizPoco(Quiz quiz) {
        this.ID = quiz.ID;
        this.Name = quiz.Name;
    }    

    public string ToJsonString() {
        return JsonSerializer.Serialize<QuizPoco>(this);
    }
}