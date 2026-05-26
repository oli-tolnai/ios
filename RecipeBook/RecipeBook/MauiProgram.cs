using Microsoft.Extensions.Logging;
using RecipeBook.Services;
using RecipeBook.ViewModels;

namespace RecipeBook;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        SQLitePCL.Batteries_V2.Init();

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton(new HttpClient());
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<MealApiService>();
        builder.Services.AddSingleton<RecipeShareService>();

        builder.Services.AddTransient<SearchViewModel>();
        builder.Services.AddTransient<RecipeDetailViewModel>();
        builder.Services.AddTransient<FavoritesViewModel>();
        builder.Services.AddTransient<MyRecipesViewModel>();
        builder.Services.AddTransient<MyRecipeEditViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
