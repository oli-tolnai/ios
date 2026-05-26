# MAUI ZH A4 kódpuska

Írd felül a namespace-t a saját projekt nevére, pl. `minta_zh_sajat`.

## 1. NuGet / using

```csharp
CommunityToolkit.Mvvm
```

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Text.Json;
```

## 2. Modell: `ShoppingListItem.cs`

```csharp
using CommunityToolkit.Mvvm.ComponentModel;

namespace minta_zh_sajat;

public partial class ShoppingListItem : ObservableObject
{
    [ObservableProperty] string name;
    [ObservableProperty] string description;
    [ObservableProperty] int quantity;
    [ObservableProperty] string imageURL;
    [ObservableProperty] bool isDone;
}
```

## 3. ViewModel: `MainPageViewModel.cs`

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace minta_zh_sajat;

[QueryProperty(nameof(NewItem), "newitem")]
public partial class MainPageViewModel : ObservableObject
{
    string path = Path.Combine(FileSystem.Current.AppDataDirectory, "shoppingdata.json");

    public ObservableCollection<ShoppingListItem> ShoppingList { get; private set; }

    [ObservableProperty]
    ShoppingListItem selectedItem;

    public ShoppingListItem NewItem
    {
        set { ShoppingList.Add(value); }
    }

    public MainPageViewModel()
    {
        ShoppingList = new ObservableCollection<ShoppingListItem>();
    }

    [RelayCommand]
    async Task AddNewAsync()
    {
        await Shell.Current.GoToAsync("newitem");
    }

    [RelayCommand]
    void DeleteSelected()
    {
        if (SelectedItem != null)
        {
            ShoppingList.Remove(SelectedItem);
            SelectedItem = null;
        }
    }

    [RelayCommand]
    async Task ShareSelected()
    {
        if (SelectedItem != null)
        {
            await Share.Default.RequestAsync(new ShareTextRequest()
            {
                Title = "Shopping Item",
                Text = $"Buy {SelectedItem.Quantity} {SelectedItem.Name}(s)!"
            });
        }
    }

    public async Task LoadFromJson()
    {
        if (File.Exists(path) && ShoppingList.Count == 0)
        {
            string json = await File.ReadAllTextAsync(path);
            List<ShoppingListItem>? data =
                JsonSerializer.Deserialize<List<ShoppingListItem>>(json);

            if (data != null)
                data.ForEach(item => ShoppingList.Add(item));
        }
    }

    public async Task SaveToJson()
    {
        string json = JsonSerializer.Serialize(ShoppingList);
        await File.WriteAllTextAsync(path, json);
    }
}
```

## 4. `MainPage.xaml`

Fontos fejléc:

```xml
<ContentPage
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:local="clr-namespace:minta_zh_sajat"
    x:Class="minta_zh_sajat.MainPage"
    x:DataType="local:MainPageViewModel">
```

Teljes oldal váz:

```xml
<ContentPage
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:local="clr-namespace:minta_zh_sajat"
    x:Class="minta_zh_sajat.MainPage"
    x:DataType="local:MainPageViewModel">

    <ContentPage.Resources>
        <Style TargetType="Button">
            <Setter Property="BackgroundColor" Value="MidnightBlue" />
            <Setter Property="FontAttributes" Value="Bold" />
            <Setter Property="Margin" Value="5" />
        </Style>
    </ContentPage.Resources>

    <Grid RowDefinitions="*,Auto">
        <CollectionView ItemsSource="{Binding ShoppingList}"
                        SelectedItem="{Binding SelectedItem}"
                        SelectionMode="Single">
            <CollectionView.ItemTemplate>
                <DataTemplate x:DataType="local:ShoppingListItem">
                    <Border Stroke="Black" StrokeThickness="1"
                            StrokeShape="RoundRectangle 20" Margin="5">
                        <Grid ColumnDefinitions="*,*,3*">
                            <CheckBox IsChecked="{Binding IsDone}"
                                      HorizontalOptions="Center"
                                      VerticalOptions="Center" />

                            <Image Source="{Binding ImageURL}"
                                   Grid.Column="1"
                                   HeightRequest="100" />

                            <VerticalStackLayout Grid.Column="2"
                                                 VerticalOptions="Center">
                                <Label Text="{Binding Name}"
                                       FontAttributes="Bold" />
                                <Label Text="{Binding Description}" />
                                <Label Text="{Binding Quantity,
                                    StringFormat='Quantity: {0}'}" />
                            </VerticalStackLayout>
                        </Grid>
                    </Border>
                </DataTemplate>
            </CollectionView.ItemTemplate>
        </CollectionView>

        <HorizontalStackLayout Grid.Row="1" Spacing="10">
            <Button Text="Add New Item"
                    Command="{Binding AddNewCommand}" />
            <Button Text="Delete Selected"
                    Command="{Binding DeleteSelectedCommand}" />
            <Button Text="Share Selected"
                    Command="{Binding ShareSelectedCommand}" />
        </HorizontalStackLayout>
    </Grid>
</ContentPage>
```

## 5. `MainPage.xaml.cs`

```csharp
namespace minta_zh_sajat;

public partial class MainPage : ContentPage
{
    MainPageViewModel viewModel;

    public MainPage(MainPageViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await viewModel.LoadFromJson();
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        await viewModel.SaveToJson();
    }
}
```

## 6. `NewItemPage.xaml`

```xml
<ContentPage
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    x:Class="minta_zh_sajat.NewItemPage"
    Title="New Shopping List Item">

    <VerticalStackLayout Padding="10">
        <Label Text="Name" />
        <Entry Text="{Binding Name}" />

        <Label Text="Description" />
        <Entry Text="{Binding Description}" />

        <Label Text="Quantity" />
        <Entry Text="{Binding Quantity}" Keyboard="Numeric" />

        <Label Text="Image" />
        <Button Text="Select image"
                Clicked="SelectImage_Clicked" />

        <Image Source="{Binding ImageURL}"
               WidthRequest="200"
               HeightRequest="200" />

        <Button Text="Save"
                Clicked="Save_Clicked" />
    </VerticalStackLayout>
</ContentPage>
```

## 7. `NewItemPage.xaml.cs`

```csharp
namespace minta_zh_sajat;

public partial class NewItemPage : ContentPage
{
    public ShoppingListItem NewItem { get; set; }

    public NewItemPage()
    {
        InitializeComponent();
        NewItem = new ShoppingListItem();
        BindingContext = NewItem;
    }

    private async void Save_Clicked(object sender, EventArgs e)
    {
        var param = new ShellNavigationQueryParameters()
        {
            { "newitem", NewItem }
        };

        await Shell.Current.GoToAsync("..", param);
    }

    private async void SelectImage_Clicked(object sender, EventArgs e)
    {
        FileResult? photo = await MediaPicker.Default.PickPhotoAsync();

        if (photo != null)
        {
            string localFilePath = Path.Combine(
                FileSystem.Current.AppDataDirectory,
                photo.FileName);

            if (!File.Exists(localFilePath))
            {
                using Stream sourceStream = await photo.OpenReadAsync();
                using FileStream localFileStream = File.OpenWrite(localFilePath);
                await sourceStream.CopyToAsync(localFileStream);
            }

            NewItem.ImageURL = localFilePath;
        }
    }
}
```

## 8. `AppShell.xaml`

```xml
<Shell
    x:Class="minta_zh_sajat.AppShell"
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:local="clr-namespace:minta_zh_sajat"
    Shell.FlyoutBehavior="Flyout">

    <ShellContent
        Title="Shopping List"
        ContentTemplate="{DataTemplate local:MainPage}"
        Route="MainPage" />

    <ShellContent
        Title="About"
        ContentTemplate="{DataTemplate local:AboutPage}"
        Route="About" />
</Shell>
```

## 9. `AppShell.xaml.cs`

```csharp
namespace minta_zh_sajat;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("newitem", typeof(NewItemPage));
    }
}
```

## 10. `AboutPage.xaml`

```xml
<ContentPage
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    x:Class="minta_zh_sajat.AboutPage"
    Title="About">

    <VerticalStackLayout>
        <Label Text="This is the best test!"
               HorizontalOptions="Center"
               VerticalOptions="Center" />
    </VerticalStackLayout>
</ContentPage>
```

## 11. `AboutPage.xaml.cs`

```csharp
namespace minta_zh_sajat;

public partial class AboutPage : ContentPage
{
    public AboutPage()
    {
        InitializeComponent();
    }
}
```

## 12. `MauiProgram.cs`

```csharp
using Microsoft.Extensions.Logging;

namespace minta_zh_sajat;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<MainPageViewModel>();
        builder.Services.AddSingleton<MainPage>();

        return builder.Build();
    }
}
```

## 13. Ha Windows futtatási profil hiba van

`Properties/launchSettings.json`:

```json
{
  "profiles": {
    "Windows Machine": {
      "commandName": "Project",
      "nativeDebugging": false
    }
  }
}
```

## 14. Mini emlékeztető

```text
[ObservableProperty] string name; -> Name
[RelayCommand] AddNewAsync() -> AddNewCommand
ObservableCollection -> UI frissül
BindingContext = viewModel
Route: RegisterRoute("newitem", typeof(NewItemPage))
Vissza: GoToAsync("..", { "newitem", NewItem })
Mentés: OnDisappearing
Betöltés: OnAppearing
```
