namespace minta_zh_sajat
{
    public partial class MainPage : ContentPage
    {

        MainPageViewModel viewModel;

        public MainPage(MainPageViewModel viewmodel)
        {
            InitializeComponent();
            this.viewModel = viewmodel;
            this.BindingContext = viewmodel;
        }
    }
}
