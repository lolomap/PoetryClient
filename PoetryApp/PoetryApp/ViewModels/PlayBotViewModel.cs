using PoetryApp.Models;
using PoetryApp.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PoetryApp.ViewModels
{
	public class PlayBotViewModel : BaseViewModel, INotifyPropertyChanged
	{
		public string SendMessageInputText { set; get; }
		public ObservableCollection<Message> Messages { get; }
		public bool HintWindowVisibility { get => _hintwindowvisibility; set { _hintwindowvisibility = value; NotifyPropertyChanged(); } }
		private bool _hintwindowvisibility = false;

		private string MessagesText = "";

		PoemAnalyzer analyzer;

		public new event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public PlayBotViewModel()
		{
			Title = "Игра с ботом";
			Messages = new ObservableCollection<Message>();
			SendMessageCommand = new Command(async () => await OnSendMessage());
			ShowHintWindowCommand = new Command(OnShowHintWindow);
			HideHintWindowCommand = new Command(OnHideHintWindow);

			analyzer = new PoemAnalyzer();

			Messages.Add(new Message("Поэт", "" + Task.Run(() => GenerationAPI.GeneratePorfire("Белеет парус одинокий,\\n")).Result));
		}

		public ICommand SendMessageCommand { get; }
		public ICommand ShowHintWindowCommand { get; }
		public ICommand HideHintWindowCommand { get; }

		private void OnShowHintWindow()
		{
			HintWindowVisibility = true;
		}
		private void OnHideHintWindow()
		{
			HintWindowVisibility = false;
		}

		private async Task OnSendMessage()
		{
			if (SendMessageInputText == "" || SendMessageInputText == null)
				return;
			SendMessageInputText = await DictionaryAPIManager.SpellCheck(SendMessageInputText);
			Message m = new Message("Игрок", SendMessageInputText, true);
			Messages.Add(m);
			MessagesText += m.Text + "\\n";
			var m1 = SendMessageInputText.Trim();
			var m2 = Messages[Messages.Count - 2].Text.Trim();
			m.Score = await analyzer.ScoreRhyme(m1, m2);

			await BotGenMessage();

			//SendMessageInputText = "";
		}

		private async Task BotGenMessage()
		{
			Message m = new Message("Поэт", "...");
			Messages.Add(m);
			m.Text = await GenerationAPI.GeneratePorfire(MessagesText);
			MessagesText += m.Text + "\\n";
		}

	}
}