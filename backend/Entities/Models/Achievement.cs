using System.ComponentModel.DataAnnotations;

public class Achievement
{
    public Guid ID { get; set; } 

    [Required]
    public string? Name { get; set; }

    [Required]
    public string? Description { get; set; }

    public string Picture { get; set; } = "";
}