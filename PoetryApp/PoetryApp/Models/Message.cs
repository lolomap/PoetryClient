using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace PoetryApp.Models
{
	public class Message : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		// This method is called by the Set accessor of each property.
		// The CallerMemberName attribute that is applied to the optional propertyName
		// parameter causes the property name of the caller to be substituted as an argument.
		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
	{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}


		public string Text { get; set; }
		public bool isPlayer { get; set; }
		public string FromName { get; set; }
		public string ScoreText { get => _scoreText; set { _scoreText = value; NotifyPropertyChanged("ScoreText"); } }
		string _scoreText;
		public double Score { get { return score_; } set { ScoreText = value.ToString(); score_ = value; } }
		double score_;

		public LayoutOptions HorizontalOptions { get; set; }

		public Message(string from, string text, bool isplayer = false)
		{
			Text = text;
			FromName = from;
			isPlayer = isplayer;

			if (isplayer)
				HorizontalOptions = LayoutOptions.End;
			else
				HorizontalOptions = LayoutOptions.Start;

			Score = 0;
		}
	}
}
