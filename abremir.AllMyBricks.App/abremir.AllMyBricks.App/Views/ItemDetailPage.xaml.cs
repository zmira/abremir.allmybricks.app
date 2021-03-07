using abremir.AllMyBricks.App.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace abremir.AllMyBricks.App.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}