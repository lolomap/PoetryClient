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
	public class PlayBotViewModel : BaseViewModel, INotifyPropertyChanged
	{
		public string SendMessageInputText { set; get; }
		public ObservableCollection<Message> Messages { get; }
		public bool HintWindowVisibility { get => _hintwindowvisibility; set { _hintwindowvisibility = value; NotifyPropertyChanged(); } }
		private bool _hintwindowvisibility = false;
		public bool ResultsWindowVisibility { get => _resultswindowvisibility; set { _resultswindowvisibility = value; NotifyPropertyChanged(); } }
		private bool _resultswindowvisibility = false;

		public string MessagesText { get => _messagetext; set { _messagetext = value; NotifyPropertyChanged(); } }
		private string _messagetext = "";

		public string TotalScoreText { get => "Счёт: " + TotalScore.ToString(); private set { NotifyPropertyChanged(); } }
		public double TotalScore { get => _totalscore; set { _totalscore = value; NotifyPropertyChanged(); } }
		private double _totalscore = 0;

		/*public bool MainPageWindowVisibility { get => _mainpagewindowvisibility; set { _mainpagewindowvisibility = value; NotifyPropertyChanged(); } }
		private bool _mainpagewindowvisibility = false;*/


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
			ShowResultsWindowCommand = new Command(OnShowResultsWindow);
			//HideResultsWindowCommand = new Command(OnHideResultsWindow);
			SharePoemCommand = new Command(async () => await OnSharePoem());
			BackButtonPressed = new Command(async () => await OnBackButtonPressed());
			//BackButtonNotPressed = new Command(async () => await OnBackButtonNotPressed());

			analyzer = new PoemAnalyzer();

			_ = DictionaryAPIManager.SearchWordInDictionary("проверка");

			//string starttext = Task.Run(() => GenerationAPI.GeneratePorfire("Белеет парус одинокий,\n")).Result;
			string starttext = Task.Run(() => GenerationAPI.GenerateSimple()).Result;
			//string starttext = await GenerationAPI.GenerateSimple();
			MessagesText += starttext + "\n";
			Messages.Add(new Message("Поэт", starttext));
		}

		public ICommand SendMessageCommand { get; }
		public ICommand ShowHintWindowCommand { get; }
		public ICommand HideHintWindowCommand { get; }
		//public ICommand HideResultsWindowCommand { get; }
		public ICommand ShowResultsWindowCommand { get; }
		public ICommand SharePoemCommand { get; }
		public ICommand BackButtonPressed { get; }
		//public ICommand BackButtonNotPressed { get; }

		private void OnShowHintWindow()
		{
			HintWindowVisibility = true;
		}
		private void OnHideHintWindow()
		{
			HintWindowVisibility = false;
		}
		private void OnShowResultsWindow()
		{
			ResultsWindowVisibility = true;
		}
		/*private void OnHideResultsWindow()
		{
			ResultsWindowVisibility = false;
		}*/

		/*private void OnBackButtonPressed()
		{
			MainPageWindowVisibility = true;
		}
		private void OnBackButtonNotPressed() 
		{
			MainPageWindowVisibility = false;
		}*/

		private async Task OnBackButtonPressed()
		{
			await Application.Current.MainPage.Navigation.PopAsync();
		}


		private async Task OnSharePoem()
		{
			await Share.RequestAsync(new ShareTextRequest
			{
				Text = MessagesText + "\n\nСтихотворение получено с помощью приложения Стихоплёт",
				Title = "Share"
			});
		}

		private async Task OnSendMessage()
		{
			if (SendMessageInputText == "" || SendMessageInputText == null)
				return;
			//SendMessageInputText = await DictionaryAPIManager.SpellCheck(SendMessageInputText);
			Message m = new Message("Игрок", SendMessageInputText, true);
			Messages.Add(m);
			MessagesText += m.Text + "\n";
			string m1 = SendMessageInputText.Trim();
			string m2 = Messages[Messages.Count - 2].Text.Trim();
			Tuple<double, Word> result = await analyzer.ScoreRhyme(m1, m2);
			double score = result.Item1;
			m.Score = score;
			TotalScore += score;

			if (result.Item2 != null)
				await BotGenMessage(result.Item2.speechPartSimplified, m1);
			else
				await BotGenMessage(0, m1);

			//SendMessageInputText = "";
		}

		private async Task BotGenMessage(int speechPart, string message)
		{
			Message m = new Message("Поэт", "...");
			Messages.Add(m);
			//m.Text = await GenerationAPI.GeneratePorfire(MessagesText);
			m.Text = await GenerationAPI.GenerateRhyme(message, speechPart);
			MessagesText += m.Text + "\n";
		}

	}
}