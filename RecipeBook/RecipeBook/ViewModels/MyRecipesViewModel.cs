using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeBook.Helpers;
using RecipeBook.Models;
using RecipeBook.Services;

namespace RecipeBook.ViewModels;

public partial class MyRecipesViewModel : BaseViewModel
{
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    private ObservableCollection<MyRecipe> myRecipes = new();

    [ObservableProperty]
    private bool isEmpty;

    public MyRecipesViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        Title = "Saját receptek";
    }

    [RelayCommand]
    public async Task LoadRecipesAsync()
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            var items = await _databaseService.GetMyRecipesAsync();
            MyRecipes.Clear();
            foreach (var item in items)
            {
                MyRecipes.Add(item);
            }

            IsEmpty = MyRecipes.Count == 0;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand(AllowConcurrentExecutions = true)]
    public async Task DeleteRecipeAsync(MyRecipe? recipe)
    {
        if (recipe is null)
        {
            return;
        }

        var confirm = await Shell.Current.DisplayAlert("Törlés", $"Törlöd ezt a receptet: {recipe.Name}?", "Igen", "Mégse");
        if (!confirm)
        {
            return;
        }

        await _databaseService.DeleteMyRecipeAsync(recipe);
        await LoadRecipesAsync();
    }

    [RelayCommand(AllowConcurrentExecutions = true)]
    public async Task NavigateToEditAsync(MyRecipe? recipe)
    {
        if (recipe is null)
        {
            await Shell.Current.GoToAsync(AppRoutes.MyRecipeEditPage);
            return;
        }

        var route = $"{AppRoutes.MyRecipeEditPage}?id={recipe.Id}";
        await Shell.Current.GoToAsync(route);
    }

    [RelayCommand(AllowConcurrentExecutions = true)]
    public async Task NavigateToDetailAsync(MyRecipe? recipe)
    {
        if (recipe is null)
        {
            return;
        }

        var route = $"{AppRoutes.RecipeDetailPage}?myRecipeId={recipe.Id}&source=my";
        await Shell.Current.GoToAsync(route);
    }
}
