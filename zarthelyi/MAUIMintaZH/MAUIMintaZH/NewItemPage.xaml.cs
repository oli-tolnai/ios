using CommunityToolkit.Mvvm.Input;

namespace MAUIMintaZH;

public partial class NewItemPage : ContentPage
{
    public ShoppingListItem NewItem { get; set; }
    public NewItemPage()
	{
		InitializeComponent();
		NewItem = new ShoppingListItem();
		BindingContext = NewItem;
	}

    private async void Button_Clicked(object sender, EventArgs e)
    {

        var param=new ShellNavigationQueryParameters()
        {
            {"newitem",NewItem}
        };
        await Shell.Current.GoToAsync("..", param);
    }

    private void Button_Clicked_1(object sender, EventArgs e)
    {

    }

    //private async void Button_Clicked_1(object sender, EventArgs e)
    //{
    //    FileResult? photo = await MediaPicker.Default.PickPhotoAsync();

    //    if (photo != null)
    //    {
    //        string localFilePath = Path.Combine(FileSystem.Current.AppDataDirectory,  photo.FileName);
    //        if (!File.Exists(localFilePath))
    //        {
    //            using Stream sourceStream = await photo.OpenReadAsync();
    //            using FileStream localFileStream = File.OpenWrite(localFilePath);

    //            await sourceStream.CopyToAsync(localFileStream);
    //        }
    //        NewItem.ImageURL = localFilePath;
    //    }

    //}
}