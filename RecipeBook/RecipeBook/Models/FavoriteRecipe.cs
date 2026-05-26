using SQLite;

namespace RecipeBook.Models;

[Table("FavoriteRecipes")]
public class FavoriteRecipe
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull]
    public string MealId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string ThumbnailUrl { get; set; } = string.Empty;

    public string Ingredients { get; set; } = string.Empty;

    public string Instructions { get; set; } = string.Empty;

    public DateTime SavedAt { get; set; } = DateTime.Now;
}
