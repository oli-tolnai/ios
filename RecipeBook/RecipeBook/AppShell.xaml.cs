using RecipeBook.Helpers;
using RecipeBook.Views;

namespace RecipeBook;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(AppRoutes.RecipeDetailPage, typeof(RecipeDetailPage));
        Routing.RegisterRoute(AppRoutes.MyRecipeEditPage, typeof(MyRecipeEditPage));
    }
}
