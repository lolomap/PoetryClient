using PoetryApp.Models;
using PoetryApp.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PoetryApp.ViewModels
{
	public class AccountViewModel : BaseViewModel, INotifyPropertyChanged
	{
		private string _username;
		public string UserName { get => _username; set { _username = value; NotifyPropertyChanged(); } }

		private int _id;
		public int Id { get => _id; set { _id = value; NotifyPropertyChanged(); } }

		private int _totalscore;
		public int TotalScore { get => _totalscore; set { _totalscore = value; NotifyPropertyChanged(); } }

		private int _gamescount;
		public int GamesCount { get => _gamescount; set { _gamescount = value; NotifyPropertyChanged(); } }

		private bool _logged;
		public bool LoggedOut { get => _logged; set { _logged = value; NotifyPropertyChanged(); } }

		private bool _isrefreshing;
		public bool IsRefreshing { get => _isrefreshing; set { _isrefreshing = value; NotifyPropertyChanged(); } }


		public AccountViewModel()
		{
			Title = "Профиль";
			Render();

			LoginCommand = new Command(async () => await OnLogin());
			RefreshCommand = new Command(OnRefresh);
		}

		private void Render()
		{
			LoggedOut = Account.user == null;
			if (!LoggedOut)
			{
				UserName = Account.user.Name;
				Id = Account.user.Id;
				TotalScore = Account.user.TotalScore;
				GamesCount = Account.user.GamesCount;
			}
		}

		public ICommand LoginCommand { get; }
		public ICommand RefreshCommand { get; }

		public async Task OnLogin()
		{
			await Application.Current.MainPage.Navigation.PushAsync(new LoginPage());
		}

		public void OnRefresh()
		{
			Render();


			IsRefreshing = false;
		}

		public new event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}