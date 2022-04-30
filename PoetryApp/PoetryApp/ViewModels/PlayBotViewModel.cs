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
			SendMessageCommand = new Command(OnSendMessage);

			analyzer = new PoemAnalyzer();

			Messages.Add(new Message("Поэт", "Приветствую, игрок!"));
		}

		public ICommand SendMessageCommand { get; }

		private void OnSendMessage(object obj)
		{
			Message m = new Message("Игрок", SendMessageInputText, true);
			Messages.Add(m);
			//m.Score = PoemAnalyzer.CalculateRhyme(m.Text, Messages[-2].Text, PoemAnalyzer.RhymeMode.LastWord);
			//SendMessageInputText = "";
		}
	}
}