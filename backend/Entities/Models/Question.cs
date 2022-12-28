using System.ComponentModel.DataAnnotations;

public class Question
{
    public string? Description { get; set; }

    public List<string>? Options { get; set; }

    public int Answer { get; set; }

    public int Points { get; set; }
    
}