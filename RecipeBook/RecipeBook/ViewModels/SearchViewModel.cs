using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeBook.Helpers;
using RecipeBook.Models;
using RecipeBook.Services;

namespace RecipeBook.ViewModels;

public partial class SearchViewModel : BaseViewModel
{
    private readonly MealApiService _mealApiService;
    private int _searchRequestVersion;

    [ObservableProperty]
    private string searchQuery = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Meal> searchResults = new();

    [ObservableProperty]
    private bool hasNoResults;

    [ObservableProperty]
    private bool isOffline;

    [ObservableProperty]
    private bool canSearch = true;

    public SearchViewModel(MealApiService mealApiService)
    {
        _mealApiService = mealApiService;
        Title = "Keresés";

        CheckConnectivity();
        Connectivity.ConnectivityChanged += (_, _) => CheckConnectivity();
    }

    partial void OnIsOfflineChanged(bool value)
    {
        CanSearch = !value;
    }

    [RelayCommand]
    public void CheckConnectivity()
    {
        IsOffline = Connectivity.NetworkAccess != NetworkAccess.Internet;
        if (IsOffline)
        {
            HasNoResults = SearchResults.Count == 0;
        }
    }

    [RelayCommand]
    public async Task SearchAsync()
    {
        CheckConnectivity();

        if (IsOffline)
        {
            await Shell.Current.DisplayAlert("Offline", "Nincs internetkapcsolat, keresés most nem elérhető.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(SearchQuery))
        {
            SearchResults.Clear();
            HasNoResults = false;
            return;
        }

        if (IsBusy)
        {
            return;
        }

        var query = SearchQuery.Trim();
        var currentRequestVersion = Interlocked.Increment(ref _searchRequestVersion);

        try
        {
            IsBusy = true;
            var meals = await _mealApiService.SearchMealsAsync(query);

            // If a newer search already started, ignore this outdated response.
            if (currentRequestVersion != _searchRequestVersion)
            {
                return;
            }

            var filteredMeals = meals
                .Where(m => !string.IsNullOrWhiteSpace(m.StrMeal)
                            && m.StrMeal.Contains(query, StringComparison.OrdinalIgnoreCase))
                .OrderBy(m => m.StrMeal)
                .ToList();

            SearchResults.Clear();
            foreach (var meal in filteredMeals)
            {
                SearchResults.Add(meal);
            }

            HasNoResults = filteredMeals.Count == 0;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand(AllowConcurrentExecutions = true)]
    public async Task OpenDetailAsync(Meal? meal)
    {
        if (meal is null || string.IsNullOrWhiteSpace(meal.IdMeal))
        {
            return;
        }

        var route = $"{AppRoutes.RecipeDetailPage}?mealId={Uri.EscapeDataString(meal.IdMeal)}&source=api";
        await Shell.Current.GoToAsync(route);
    }
}
