using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zarthalyiPuska
{
    public partial class ShoppingItem : ObservableObject 
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
