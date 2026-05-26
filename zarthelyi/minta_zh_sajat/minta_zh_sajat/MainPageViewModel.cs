using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minta_zh_sajat
{
    [QueryProperty(nameof(NewItem), "NewItem")]
    public partial class MainPageViewModel : ObservableObject
    {
        public ObservableCollection<shoppingListItem> ShoppingList { get; private set; }

        public MainPageViewModel()
        {
            ShoppingList = new ObservableCollection<shoppingListItem>();
        }

        [ObservableProperty]
        shoppingListItem selectedItem;


        [RelayCommand]
        async Task AddAsync()
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

        public shoppingListItem NewItem 
        {
            set { ShoppingList.Add(value); }
        }

    }
}
