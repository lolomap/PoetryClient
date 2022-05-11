using PoetryApp.Models;
using PoetryApp.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PoetryApp.ViewModels
{
	public class PlayBotViewModel : BaseViewModel
	{
		public string SendMessageInputText { set; get; }
		public ObservableCollection<Message> Messages { get; }

		PoemAnalyzer analyzer;

		public PlayBotViewModel()
		{
			Title = "Игра с ботом";
			Messages = new ObservableCollection<Message>();
			//SendMessageCommand = new Command(async () => await OnSendMessage());
			SendMessageCommand = new Command(async () => await OnSendMessage());

			analyzer = new PoemAnalyzer();

			Messages.Add(new Message("Поэт", "Приветствую вас! " + Task.Run(() => GenerationAPI.GeneratePorfire("Приветствую вас")).Result));
		}

		public ICommand SendMessageCommand { get; }

		private async Task OnSendMessage()
		{
			Message m = new Message("Игрок", SendMessageInputText, true);
			Messages.Add(m);
			var m1 = SendMessageInputText.Split(' ');
			var m2 = Messages[Messages.Count - 2].Text.Split(' ');
			m.Score = await analyzer.ScoreRhyme(m1[m1.Length - 1], m2[m2.Length - 1]);
			//SendMessageInputText = "";
		}

	}
}