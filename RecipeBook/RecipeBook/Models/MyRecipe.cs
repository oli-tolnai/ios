using SQLite;

namespace RecipeBook.Models;

[Table("MyRecipes")]
public class MyRecipe
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull]
    public string Name { get; set; } = string.Empty;

    public string Ingredients { get; set; } = string.Empty;

    public string Instructions { get; set; } = string.Empty;

    public string? PhotoPath { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }
}
