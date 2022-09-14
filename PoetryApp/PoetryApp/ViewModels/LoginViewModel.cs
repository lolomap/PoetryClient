using PoetryApp.Models;
using PoetryApp.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PoetryApp.ViewModels
{
	public class LoginViewModel : BaseViewModel
	{
		private string _username;
		private string _password;

		public string Username { get { return _username; } set { _username = value; NotifyPropertyChanged(); } }
		public string Password { get { return _password; } set { _password = value; NotifyPropertyChanged(); } }

		public ICommand LoginCommand { get; }
		public ICommand RegisterCommand { get; }

		public LoginViewModel()
		{
			LoginCommand = new Command(async () => await OnLogin());
			RegisterCommand = new Command(async () => await OnRegister());
		}

		private async Task OnLogin()
		{
			if (await Account.Login(Username, Password) == 0)
			{
				await Application.Current.MainPage.Navigation.PopAsync();
			}

		}
		private async Task OnRegister()
		{
			if (await Account.Register(Username, Password) == 0)
			{
				await Application.Current.MainPage.Navigation.PopAsync();
			}
		}

		public new event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

	}
}