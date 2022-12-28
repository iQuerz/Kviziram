using System.ComponentModel.DataAnnotations;

public class Quiz
{
    public Guid ID { get; set; } 

    [Required]
    public string? Name { get; set; }

    public Quiz() {
        this.ID = Guid.NewGuid();
        this.Name = "";
    }
}