using CommunityToolkit.Mvvm.Messaging;

namespace MAUIMintaZH
{
    public partial class MainPage : ContentPage
    {
        MainPageViewModel viewModel;
        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            BindingContext=viewModel;
            //WeakReferenceMessenger.Default.Register<string>(this,(async (r, m) => await Shell.Current.DisplayAlert("Warning",m,"OK") ));
        }

        

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            await viewModel.SaveToJson();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await viewModel.LoadFromJson();
        }
    }

}
