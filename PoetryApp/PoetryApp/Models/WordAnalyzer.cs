using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Iveonik.Stemmers;

namespace PoetryApp.Models
{
	public class WordAnalyzer
	{
		RussianStemmer stemmer = new RussianStemmer();

		public enum SpeechPart
		{
			Adjective,
			Verb,
			Noun,
			Participle, //Причастие
			Conjuction
		}

		private const string VOWEL = "аеиоуыэюя";
		private const string PERFECTIVEGROUND = "((ив|ивши|ившись|ыв|ывши|ывшись)|((?<=[ая])(в|вши|вшись)))$";
		private const string REFLEXIVE = "(с[яь])$";
		private const string ADJECTIVE = "(ее|ие|ые|ое|ими|ыми|ей|ий|ый|ой|ем|им|ым|ом|его|ого|еых|ую|юю|ая|яя|ою|ею)$";
		private const string PARTICIPLE = "((ивш|ывш|ующ)|((?<=[ая])(ем|нн|вш|ющ|щ)))$";
		private const string VERB = "((ила|ыла|ена|ейте|уйте|ите|или|ыли|ей|уй|ил|ыл|им|ым|ены|ить|ыть|ишь|ую|ю)|((?<=[ая])(ла|на|ете|йте|ли|й|л|ем|н|ло|но|ет|ют|ны|ть|ешь|нно)))$";
		private const string NOUN = "(а|ев|ов|ие|ье|е|иями|ями|ами|еи|ии|и|ией|ей|ой|ий|й|и|ы|ь|ию|ью|ю|ия|ья|я)$";
		private const string RVRE = "^(.*?[аеиоуыэюя])(.*)$";
		private const string DERIVATIONAL = "[^аеиоуыэюя][аеиоуыэюя]+[^аеиоуыэюя]+[аеиоуыэюя].*(?<=о)сть?$";
		private const string SUPERLATIVE = "(ейше|ейш)?";

		public string Stemm(string word)
		{
			word = word.ToLower();
			word = word.Replace("ё", "е");
			if (IsMatch(word, RVRE))
			{

				if (!Replace(ref word, PERFECTIVEGROUND, ""))
				{
					Replace(ref word, REFLEXIVE, "");
					if (Replace(ref word, ADJECTIVE, ""))
					{
						Replace(ref word, PARTICIPLE, "");
					}
					else
					{
						if (!Replace(ref word, VERB, ""))
						{
							Replace(ref word, NOUN, "");
						}

					}

				}


				Replace(ref word, "и$", "");

				if (IsMatch(word, DERIVATIONAL))
				{
					Replace(ref word, "ость?$", "");
				}


				if (!Replace(ref word, "ь$", ""))
				{
					Replace(ref word, SUPERLATIVE, "");
					Replace(ref word, "нн$", "н");
				}

			}

			return word;
		}

		private bool IsMatch(string word, string matchingPattern)
		{
			return new Regex(matchingPattern).IsMatch(word);
		}

		private bool Replace(ref string replace, string cleaningPattern, string by)
		{
			string original = replace;
			replace = new Regex(cleaningPattern,
						RegexOptions.ExplicitCapture |
						RegexOptions.Singleline
						).Replace(replace, by);
			return original != replace;
		}


		public string NuStemm(string a)
		{
			if (stemmer.CanStem())
				return stemmer.Stem(a);
			else return a;
		}

		public string GetBase(string a)
		{
			//Сделать словарь суффиксов и приставок и отделить основную часть (NuStemm) от них, чтобы получить корень
			return "";
		}

		public SpeechPart GetSpeechPart(string w)
		{
			w = w.ToLower();
			w = w.Replace("ё", "е");

			if (w.Length < 3)
				return SpeechPart.Conjuction; //добавить проверку на местоимения

			if (w.Substring(w.Length - 2) == "ся" || w.Substring(w.Length - 2) == "сь")
			{
				return SpeechPart.Verb;
			}

			//КАК ОТЛИЧИТЬ ГЛАГОЛ ОТ СУЩ И ПРИЛ??

			//string end = w.Substring(NuStemm(w).Length);
			if (IsMatch(w, RVRE))
			{
				if (IsMatch(w, ADJECTIVE))
				{
					string ww = w;
					Replace(ref ww, ADJECTIVE, "");
					if (IsMatch(ww, PARTICIPLE))
						return SpeechPart.Participle;
					else return SpeechPart.Adjective;
				}
				else if (IsMatch(w, VERB))
					return SpeechPart.Verb;
				else if (IsMatch(w, NOUN))
					return SpeechPart.Noun;
			}

			return SpeechPart.Noun;
		}
	}
}
