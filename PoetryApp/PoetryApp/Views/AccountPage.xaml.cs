using PoetryApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PoetryApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AccountPage : ContentPage
	{
		public AccountPage()
		{
			InitializeComponent();
			//this.BindingContext = new AccountViewModel();
			//{
			//	LoginCommand = new Command(async () => await AccountViewModel.OnLogin())
			//};
		}
	}
}