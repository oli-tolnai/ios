using System.Net.Http.Json;
using RecipeBook.Models;

namespace RecipeBook.Services;

public class MealApiService
{
    private readonly HttpClient _httpClient;

    public MealApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://www.themealdb.com/api/json/v1/1/");
    }

    public async Task<List<Meal>> SearchMealsAsync(string query)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<MealResponse>($"search.php?s={Uri.EscapeDataString(query)}");
            return response?.Meals ?? new List<Meal>();
        }
        catch
        {
            return new List<Meal>();
        }
    }

    public async Task<Meal?> GetMealByIdAsync(string mealId)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<MealResponse>($"lookup.php?i={Uri.EscapeDataString(mealId)}");
            return response?.Meals?.FirstOrDefault();
        }
        catch
        {
            return null;
        }
    }
}
