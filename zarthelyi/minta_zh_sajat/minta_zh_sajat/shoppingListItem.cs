using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minta_zh_sajat
{
    public partial class shoppingListItem:ObservableObject
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
        bool isBought;
    }
}
