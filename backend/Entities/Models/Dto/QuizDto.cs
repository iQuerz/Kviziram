using System.ComponentModel.DataAnnotations;
using System.Text.Json;

public class QuizDto
{
    public Guid ID { get; set; } 

    public string? Name { get; set; }

    public float? AvgRating { get; set; }

}