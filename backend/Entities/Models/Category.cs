using System.ComponentModel.DataAnnotations;

public class Category
{
    public Guid ID { get; set; } 

    [Required]
    public string? Name { get; set; }

    public Category() {
        this.ID = Guid.NewGuid();
        this.Name = "";
    }
}