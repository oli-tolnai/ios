using RecipeBook.Helpers;
using RecipeBook.ViewModels;

namespace RecipeBook.Views;

public partial class SearchPage : ContentPage
{
    private readonly SearchViewModel _viewModel;

    public SearchPage()
    {
        InitializeComponent();
        _viewModel = ServiceHelper.GetService<SearchViewModel>();
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.CheckConnectivityCommand.Execute(null);
    }
}
