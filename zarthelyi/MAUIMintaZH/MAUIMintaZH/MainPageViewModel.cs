using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;

namespace MAUIMintaZH
{
    [QueryProperty(nameof(NewItem), "newitem")]
    public partial class MainPageViewModel : ObservableObject
    {
        string path = Path.Combine(FileSystem.Current.AppDataDirectory, "shoppingdata.json");
         

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
            if (SelectedItem != null) {
                ShoppingList.Remove(SelectedItem);
                SelectedItem = null;
            } else
            {
                WeakReferenceMessenger.Default.Send("Select an item to delete!");
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
            } else
            {
                WeakReferenceMessenger.Default.Send("Select an item to share!");
            }
        }

        public async Task LoadFromJson()
        {
            if (File.Exists(path) && ShoppingList.Count==0)
            {
                string jsonstring = await File.ReadAllTextAsync(path);
                List<ShoppingListItem> jsonData = JsonSerializer.Deserialize<List<ShoppingListItem>>(jsonstring);
                jsonData.ForEach(item => ShoppingList.Add(item));
                
            }
        }
        

        public async Task SaveToJson()
        {
            string jsonstring = JsonSerializer.Serialize(ShoppingList);
            await File.WriteAllTextAsync(path, jsonstring);
        }
    }
}
