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

		private string MessagesText = "";

		PoemAnalyzer analyzer;

		public PlayBotViewModel()
		{
			Title = "Игра с ботом";
			Messages = new ObservableCollection<Message>();
			SendMessageCommand = new Command(async () => await OnSendMessage());

			analyzer = new PoemAnalyzer();

			Messages.Add(new Message("Поэт", "" + Task.Run(() => GenerationAPI.GeneratePorfire("Белеет парус одинокий")).Result));
		}

		public ICommand SendMessageCommand { get; }

		private async Task OnSendMessage()
		{
			Message m = new Message("Игрок", SendMessageInputText, true);
			Messages.Add(m);
			MessagesText += m.Text + "\\n";
			var m1 = SendMessageInputText.Trim().Split(' ');
			var m2 = Messages[Messages.Count - 2].Text.Trim().Split(' ');
			m.Score = await analyzer.ScoreRhyme(m1[m1.Length - 1], m2[m2.Length - 1]);

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