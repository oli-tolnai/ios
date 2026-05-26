using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MAUIMintaZH
{
    public partial class ShoppingListItem:ObservableObject
    {
        [ObservableProperty]
        string name;

        [ObservableProperty]
        string description;

        [ObservableProperty]
        int quantity;

        [ObservableProperty]
        string imageURL;

        [ObservableProperty]
        bool isDone;

    }
}
