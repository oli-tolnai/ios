using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using RecipeBook.Models;
using RecipeBook.Services;

namespace RecipeBook.ViewModels;

public partial class RecipeDetailViewModel : BaseViewModel, IQueryAttributable
{
    private readonly MealApiService _mealApiService;
    private readonly DatabaseService _databaseService;
    private readonly RecipeShareService _recipeShareService;

    private string? _mealId;
    private int? _myRecipeId;
    private string _source = "api";

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string category = string.Empty;

    [ObservableProperty]
    private string ingredients = string.Empty;

    [ObservableProperty]
    private string instructions = string.Empty;

    [ObservableProperty]
    private string imageSource = string.Empty;

    [ObservableProperty]
    private bool isSaved;

    [ObservableProperty]
    private bool canToggleFavorite;

    [ObservableProperty]
    private string favoriteButtonText = "Kedvencekhez adás";

    public RecipeDetailViewModel(
        MealApiService mealApiService,
        DatabaseService databaseService,
        RecipeShareService recipeShareService)
    {
        _mealApiService = mealApiService;
        _databaseService = databaseService;
        _recipeShareService = recipeShareService;
        Title = "Recept";
    }

    partial void OnIsSavedChanged(bool value)
    {
        FavoriteButtonText = value ? "Eltávolítás kedvencekből" : "Kedvencekhez adás";
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        _mealId = ReadQueryValue(query, "mealId");
        _source = ReadQueryValue(query, "source") ?? "api";

        var myRecipeIdValue = ReadQueryValue(query, "myRecipeId");
        _myRecipeId = int.TryParse(myRecipeIdValue, out var parsedId) ? parsedId : null;

        _ = LoadAsync();
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            Name = string.Empty;
            Category = string.Empty;
            Ingredients = string.Empty;
            Instructions = string.Empty;
            ImageSource = string.Empty;
            IsSaved = false;

            if (_myRecipeId.HasValue)
            {
                await LoadMyRecipeAsync(_myRecipeId.Value);
                return;
            }

            if (string.IsNullOrWhiteSpace(_mealId))
            {
                return;
            }

            if (_source == "favorite")
            {
                var favorite = await _databaseService.GetFavoriteByMealIdAsync(_mealId);
                if (favorite is not null)
                {
                    ApplyFavorite(favorite);
                }
                else
                {
                    await LoadFromApiAsync(_mealId);
                }
            }
            else
            {
                await LoadFromApiAsync(_mealId);
            }

            IsSaved = await _databaseService.IsFavoriteAsync(_mealId);
            CanToggleFavorite = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task ToggleFavoriteAsync()
    {
        if (!CanToggleFavorite || string.IsNullOrWhiteSpace(_mealId) || string.IsNullOrWhiteSpace(Name))
        {
            return;
        }

        if (IsSaved)
        {
            await _databaseService.DeleteFavoriteAsync(_mealId);
            IsSaved = false;
            return;
        }

        var favorite = new FavoriteRecipe
        {
            MealId = _mealId,
            Name = Name,
            Category = Category,
            Ingredients = Ingredients,
            Instructions = Instructions,
            ThumbnailUrl = ImageSource
        };

        await _databaseService.AddFavoriteIfNotExistsAsync(favorite);
        IsSaved = true;
    }

    [RelayCommand]
    public async Task ShareRecipeAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            return;
        }

        await _recipeShareService.ShareRecipeAsync(Name, Ingredients, Instructions, ImageSource);
    }

    [RelayCommand]
    public async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    private async Task LoadFromApiAsync(string mealId)
    {
        var meal = await _mealApiService.GetMealByIdAsync(mealId);
        if (meal is null)
        {
            await Shell.Current.DisplayAlert("Hiba", "Nem sikerült betölteni a receptet.", "OK");
            return;
        }

        Name = meal.StrMeal;
        Category = meal.StrCategory;
        Ingredients = meal.BuildIngredientsText();
        Instructions = meal.StrInstructions;
        ImageSource = meal.StrMealThumb;
    }

    private async Task LoadMyRecipeAsync(int myRecipeId)
    {
        var recipe = await _databaseService.GetMyRecipeByIdAsync(myRecipeId);
        if (recipe is null)
        {
            await Shell.Current.DisplayAlert("Hiba", "A recept nem található.", "OK");
            return;
        }

        Name = recipe.Name;
        Category = "Saját recept";
        Ingredients = recipe.Ingredients;
        Instructions = recipe.Instructions;
        ImageSource = recipe.PhotoPath ?? string.Empty;
        CanToggleFavorite = false;
        IsSaved = false;
    }

    private void ApplyFavorite(FavoriteRecipe favorite)
    {
        Name = favorite.Name;
        Category = favorite.Category;
        Ingredients = favorite.Ingredients;
        Instructions = favorite.Instructions;
        ImageSource = favorite.ThumbnailUrl;
    }

    private static string? ReadQueryValue(IDictionary<string, object> query, string key)
    {
        if (!query.TryGetValue(key, out var value) || value is null)
        {
            return null;
        }

        return Uri.UnescapeDataString(value.ToString() ?? string.Empty);
    }
}
