using PoetryApp.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace PoetryApp.Views
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