using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeBook.Helpers;
using RecipeBook.Models;
using RecipeBook.Services;

namespace RecipeBook.ViewModels;

public partial class FavoritesViewModel : BaseViewModel
{
    private readonly DatabaseService _databaseService;
    private readonly RecipeShareService _recipeShareService;

    [ObservableProperty]
    private ObservableCollection<FavoriteRecipe> favorites = new();

    [ObservableProperty]
    private bool isEmpty;

    public FavoritesViewModel(DatabaseService databaseService, RecipeShareService recipeShareService)
    {
        _databaseService = databaseService;
        _recipeShareService = recipeShareService;
        Title = "Kedvencek";
    }

    [RelayCommand]
    public async Task LoadFavoritesAsync()
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            var items = await _databaseService.GetFavoritesAsync();
            Favorites.Clear();
            foreach (var item in items)
            {
                Favorites.Add(item);
            }

            IsEmpty = Favorites.Count == 0;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand(AllowConcurrentExecutions = true)]
    public async Task DeleteFavoriteAsync(FavoriteRecipe? recipe)
    {
        if (recipe is null)
        {
            return;
        }

        var confirm = await Shell.Current.DisplayAlert("Törlés", $"Törlöd ezt a kedvencet: {recipe.Name}?", "Igen", "Mégse");
        if (!confirm)
        {
            return;
        }

        await _databaseService.DeleteFavoriteAsync(recipe);
        await LoadFavoritesAsync();
    }

    [RelayCommand(AllowConcurrentExecutions = true)]
    public async Task ShareRecipeAsync(FavoriteRecipe? recipe)
    {
        if (recipe is null)
        {
            return;
        }

        await _recipeShareService.ShareRecipeAsync(
            recipe.Name,
            recipe.Ingredients,
            recipe.Instructions,
            recipe.ThumbnailUrl);
    }

    [RelayCommand(AllowConcurrentExecutions = true)]
    public async Task NavigateToDetailAsync(FavoriteRecipe? recipe)
    {
        if (recipe is null || string.IsNullOrWhiteSpace(recipe.MealId))
        {
            return;
        }

        var route = $"{AppRoutes.RecipeDetailPage}?mealId={Uri.EscapeDataString(recipe.MealId)}&source=favorite";
        await Shell.Current.GoToAsync(route);
    }
}
