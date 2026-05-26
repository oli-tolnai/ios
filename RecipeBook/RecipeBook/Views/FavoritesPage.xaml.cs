using RecipeBook.Helpers;
using RecipeBook.ViewModels;

namespace RecipeBook.Views;

public partial class FavoritesPage : ContentPage
{
    private readonly FavoritesViewModel _viewModel;

    public FavoritesPage()
    {
        InitializeComponent();
        _viewModel = ServiceHelper.GetService<FavoritesViewModel>();
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = _viewModel.LoadFavoritesAsync();
    }
}
