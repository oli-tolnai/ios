using RecipeBook.Helpers;
using RecipeBook.ViewModels;

namespace RecipeBook.Views;

public partial class MyRecipesPage : ContentPage
{
    private readonly MyRecipesViewModel _viewModel;

    public MyRecipesPage()
    {
        InitializeComponent();
        _viewModel = ServiceHelper.GetService<MyRecipesViewModel>();
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = _viewModel.LoadRecipesAsync();
    }
}
