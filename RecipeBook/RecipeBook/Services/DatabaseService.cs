using RecipeBook.Models;
using SQLite;

namespace RecipeBook.Services;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _database;
    private readonly Task _initTask;

    public DatabaseService()
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "recipebook.db3");
        _database = new SQLiteAsyncConnection(dbPath);
        _initTask = InitAsync();
    }

    private async Task InitAsync()
    {
        await _database.CreateTableAsync<MyRecipe>();
        await _database.CreateTableAsync<FavoriteRecipe>();
    }

    private Task EnsureInitializedAsync() => _initTask;

    public async Task<List<MyRecipe>> GetMyRecipesAsync()
    {
        await EnsureInitializedAsync();
        return await _database.Table<MyRecipe>().OrderByDescending(x => x.CreatedAt).ToListAsync();
    }

    public async Task<MyRecipe?> GetMyRecipeByIdAsync(int id)
    {
        await EnsureInitializedAsync();
        return await _database.Table<MyRecipe>().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<int> SaveMyRecipeAsync(MyRecipe recipe)
    {
        await EnsureInitializedAsync();

        if (recipe.Id == 0)
        {
            recipe.CreatedAt = DateTime.Now;
            recipe.UpdatedAt = null;
            return await _database.InsertAsync(recipe);
        }

        recipe.UpdatedAt = DateTime.Now;
        return await _database.UpdateAsync(recipe);
    }

    public async Task<int> DeleteMyRecipeAsync(MyRecipe recipe)
    {
        await EnsureInitializedAsync();
        return await _database.DeleteAsync(recipe);
    }

    public async Task<List<FavoriteRecipe>> GetFavoritesAsync()
    {
        await EnsureInitializedAsync();
        return await _database.Table<FavoriteRecipe>().OrderByDescending(x => x.SavedAt).ToListAsync();
    }

    public async Task<FavoriteRecipe?> GetFavoriteByMealIdAsync(string mealId)
    {
        await EnsureInitializedAsync();
        return await _database.Table<FavoriteRecipe>().FirstOrDefaultAsync(x => x.MealId == mealId);
    }

    public async Task<bool> IsFavoriteAsync(string mealId)
    {
        await EnsureInitializedAsync();
        var count = await _database.Table<FavoriteRecipe>().CountAsync(x => x.MealId == mealId);
        return count > 0;
    }

    public async Task<bool> AddFavoriteIfNotExistsAsync(FavoriteRecipe recipe)
    {
        await EnsureInitializedAsync();

        if (await IsFavoriteAsync(recipe.MealId))
        {
            return false;
        }

        recipe.SavedAt = DateTime.Now;
        await _database.InsertAsync(recipe);
        return true;
    }

    public async Task<int> DeleteFavoriteAsync(FavoriteRecipe recipe)
    {
        await EnsureInitializedAsync();
        return await _database.DeleteAsync(recipe);
    }

    public async Task<int> DeleteFavoriteAsync(string mealId)
    {
        await EnsureInitializedAsync();
        var existing = await GetFavoriteByMealIdAsync(mealId);
        if (existing is null)
        {
            return 0;
        }

        return await _database.DeleteAsync(existing);
    }
}
