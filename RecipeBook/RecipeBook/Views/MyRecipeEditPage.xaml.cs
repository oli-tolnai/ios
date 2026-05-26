using RecipeBook.Helpers;
using RecipeBook.ViewModels;

namespace RecipeBook.Views;

public partial class MyRecipeEditPage : ContentPage
{
    public MyRecipeEditPage()
    {
        InitializeComponent();
        BindingContext = ServiceHelper.GetService<MyRecipeEditViewModel>();
    }
}
