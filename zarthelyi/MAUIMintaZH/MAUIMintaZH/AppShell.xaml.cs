namespace MAUIMintaZH
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("newitem", typeof(NewItemPage));
        }
    }
}
