using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PoetryApp.Models
{
	public class Message
	{
		public string Text { get; set; }
		public bool isPlayer { get; set; }
		public string FromName { get; set; }
		public string ScoreText { get; set; }
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
