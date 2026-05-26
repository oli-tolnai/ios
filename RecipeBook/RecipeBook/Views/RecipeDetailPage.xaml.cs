using RecipeBook.Helpers;
using RecipeBook.ViewModels;

namespace RecipeBook.Views;

public partial class RecipeDetailPage : ContentPage
{
    public RecipeDetailPage()
    {
        InitializeComponent();
        BindingContext = ServiceHelper.GetService<RecipeDetailViewModel>();
    }
}
