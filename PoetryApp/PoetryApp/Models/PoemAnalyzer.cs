using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace PoetryApp.Models
{
	public class Word
	{
		public enum SpeechPart
		{
			Unknown,
			Adjective,
			Verb,
			Noun,
			Participle,
			Pronoun,
			Adverb,
			Preposition,
			Conjuction,
			Particle,
			Interjection,
			Numerable,
			Gerund
		}


		public string Lemm;
		public string Text;
		public SpeechPart speechPart;
		public int StressPosition;
		public double Frequency;

		public bool SuccessParse = true;

		public Word(string json)
		{
			if (json == "None")
			{
				SuccessParse = false;
				return;
			}

			var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
			Lemm = data["Lemm"];
			Text = data["Text"];
			speechPart = (SpeechPart)int.Parse(data["speechPart"]);
			StressPosition = int.Parse(data["StressPosition"]);
			Frequency = double.Parse(data["Frequency"], CultureInfo.InvariantCulture);
		}
	}

	public class RhymePair
	{
		public enum Type
		{
			Male,
			Female,
			Dactile,
			Hyperdactile,
			Rich,
			Poor,
			Assonance,
			Dissonance,
			Cutted,
			Yotted,
			Complex
		}


		public string word1, word2;
		public List<Type> types = new List<Type>();
		public double score = 0;
		public int stressPosition;

		public char baseCons1 = (char)0, baseCons2 = (char)0;
		public string stressedPart1 = "", stressedPart2 = "";
		public Word.SpeechPart speechPart1, speechPart2;

		public RhymePair(string a, string b)
		{
			word1 = a;
			word2 = b;
		}

		public RhymePair() { }
	}

	public class PoemAnalyzer
	{
		string alphabet = "йцукенгшщзхъфывапролджэячсмитьбюё -";
		string vowels_strong = "уыаоэ";
		string vowels_soft = "юияёе";
		string vowels = "уыаоэюияёе";
		string consonants_strong = "цнгзврлджмб";
		string consonants_soft = "йкшщхфпчст";

		Dictionary<char, char> soft_strong = new Dictionary<char, char>()
		{
			{ 'к', 'г' }, { 'ш', 'ж' }, { 'ф', 'в' }, { 'п', 'б' }, { 'с', 'з' }, { 'т', 'д' },
		};
		Dictionary<char, char> strong_soft = new Dictionary<char, char>()
		{
			{ 'г', 'к' }, { 'ж', 'ш' }, { 'в', 'ф' }, { 'б', 'п' }, { 'з', 'с' }, { 'д', 'т' },
		};

		Dictionary<string, string> replace = new Dictionary<string, string>()
		{
			{ "тс", "цц" },
			{ "тч", "чч" },
			{ "тш", "шш" },
			{ "тц", "цц" },
			{ "тщ", "щщ" },
			{ "сч", "щщ" },
		};

		Dictionary<string, List<string>> simple_rhymes = new Dictionary<string, List<string>>();


		public PoemAnalyzer()
		{
			Assembly assembly = IntrospectionExtensions.GetTypeInfo(typeof(PoemAnalyzer)).Assembly;
			using (Stream stream = assembly.GetManifestResourceStream("PoetryApp.SimpleRhymes.txt"))
			{
				using (StreamReader sr = new StreamReader(stream))
				{
					string line;
					while ((line = sr.ReadLine()) != null)
					{
						string[] words = line.ToLower().Split(new[] { " — " }, StringSplitOptions.None);

						foreach(string word in words)
						{
							List<string> ws = new List<string>(words);
							ws.Remove(word);
							simple_rhymes[word] = ws;
						}
					}
				}
			}
		}

		public void FootByPoem(string poem)
		{
			//Определяем размер стиха
		}

		public bool VowelsEquality(char a, char b)
		{
			if (a == b)
			{
				return true;
			}
			else if(vowels_soft.IndexOf(a) == vowels_strong.IndexOf(b))
			{
				return true;
			}
			else if (vowels_strong.IndexOf(a) == vowels_soft.IndexOf(b))
			{
				return true;
			}

			return false;
		}

		public string Simplificate(string text)
		{
			if (text.Length == 1)
				return text;

			string a = "";
			for (int i = 0; i < text.Length - 1; i++)
			{
				string check = "" + text[i] + text[i + 1];
				if (replace.ContainsKey(check))
				{
					a += replace[check];
					i++;
				}
				else if (i == text.Length - 2)
					a += "" + text[i] + text[i + 1];
				else a += text[i];
			}

			StringBuilder sb = new StringBuilder(a);
			for (int i = 0; i < sb.Length - 1; i++)
			{
				if (!vowels.Contains(sb[i + 1]) && strong_soft.ContainsKey(sb[i]))
					sb[i] = strong_soft[sb[i]];
			}
			a = sb.ToString();

			return a;
		}

		public string CleanPunctuation(string text)
		{
			string res = "";
			foreach (char s in text)
			{
				if (alphabet.Contains(s))
					res += s;
			}

			return res;
		}

		public string YoToYe(string text)
		{
			return text.Replace('ё', 'е');
		}

		public int RecognizeStressSyllable(string word, int pos)
		{
			int offset = 0;
			for (int i = word.Length - 1; i >= 0; i--)
			{
				if (vowels.Contains(word[i]))
				{
					offset++;
					if (i == pos)
						return offset;
				}
			}
			return -1;
		}

		public void AssignRhymeTypes(RhymePair pair)
		{
			string a = pair.word1, b = pair.word2;

			switch (pair.stressPosition)
			{
				case 1:
					pair.types.Add(RhymePair.Type.Male);
					break;
				case 2:
					pair.types.Add(RhymePair.Type.Female);
					break;
				case 3:
					pair.types.Add(RhymePair.Type.Dactile);
					break;
				default:
					pair.types.Add(RhymePair.Type.Hyperdactile);
					break;
			}

			if (pair.baseCons1 == pair.baseCons2)
			{
				pair.types.Add(RhymePair.Type.Rich);
			}
			if (pair.stressedPart1 == pair.stressedPart2 && pair.stressedPart1 != "")
			{
				pair.types.Add(RhymePair.Type.Poor);
			}
		}

		bool IsVerbishRhyme(RhymePair pair)
		{
			if (pair.speechPart1 == pair.speechPart2 &&
						(pair.speechPart1 == Word.SpeechPart.Verb || pair.speechPart1 == Word.SpeechPart.Gerund))
				return true;
			else return false;
		}

		bool IsOnlyStressRhyme(RhymePair pair)
		{
			if (!pair.types.Contains(RhymePair.Type.Rich) && !pair.types.Contains(RhymePair.Type.Poor))
				return true;
			else return false;
		}

		bool IsRichRhyme(RhymePair pair)
		{
			if (pair.types.Contains(RhymePair.Type.Rich))
				return true;
			else return false;
		}

		bool IsDifferentSpeechPart(RhymePair pair)
		{
			if (pair.speechPart1 != pair.speechPart2)
				return true;
			else return false;
		}

		bool IsNonRhymingLemms(Word w1, Word w2, RhymePair pair)
		{
			if (!HasRhyme(w1.Lemm, w2.Lemm,
					RecognizeStressSyllable(w1.Lemm, w1.StressPosition),
					RecognizeStressSyllable(w2.Lemm, w2.StressPosition),
					w1.StressPosition, w2.StressPosition, pair))
				return true;
			else return false;
		}

		bool SameEndRhyme(string word1, string word2, int stressPos1, int stressPos2, RhymePair pair)
		{
			if ((word1.Length == 1 && stressPos2 == word2.Length - 1)
					|| (word2.Length == 1 && stressPos1 == word1.Length - 1))
				return true;
			else if ((stressPos1 == word1.Length - 1 && pair.stressedPart2 != "")
					|| (stressPos2 == word2.Length - 1 && pair.stressedPart1 != ""))
				return false;
			else return true;
		}

		bool HasRhyme(string w1, string w2, int stress1, int stress2, int stressPos1, int stressPos2, RhymePair pair)
		{
			if (stress1 == stress2 && VowelsEquality(w1[stressPos1], w2[stressPos2]) && SameEndRhyme(w1, w2, stressPos1, stressPos2, pair))
				return true;
			else return false;
		}

		bool IsSameLength(int l1, int l2)
		{
			return l1 == l2;
		}

		bool IsExoticWord(Word w1)
		{
			return w1.Frequency < 0.1;
		}

		bool IsSimpleRhyme(Word w1, Word w2)
		{

			if (simple_rhymes.ContainsKey(w1.Lemm))
			{
				if (simple_rhymes[w1.Lemm].Contains(w2.Lemm) || simple_rhymes[w1.Lemm].Contains(w2.Text))
					return true;
				else return false;
			}
			else if (simple_rhymes.ContainsKey(w2.Lemm))
			{
				if (simple_rhymes[w2.Lemm].Contains(w1.Lemm) || simple_rhymes[w2.Lemm].Contains(w1.Text))
					return true;
				else return false;
			}
			else if (simple_rhymes.ContainsKey(w1.Text))
			{
				if (simple_rhymes[w1.Text].Contains(w2.Lemm) || simple_rhymes[w1.Text].Contains(w2.Text))
					return true;
				else return false;
			}
			else if (simple_rhymes.ContainsKey(w2.Text))
			{
				if (simple_rhymes[w2.Text].Contains(w1.Lemm) || simple_rhymes[w2.Text].Contains(w1.Text))
					return true;
				else return false;
			}
			else return false;
		}

		void SetRhymePair(string word1, string word2, int stressPos1, int stressPos2, RhymePair pair)
		{
			if (stressPos1 < word1.Length && stressPos2 < word2.Length)
			{
				pair.stressedPart1 = word1.Substring(stressPos1 + 1);
				pair.stressedPart2 = word2.Substring(stressPos2 + 1);
			}
			if (stressPos1 > 0 && stressPos2 > 0)
			{
				if (consonants_strong.Contains(word1[stressPos1 - 1]) || consonants_soft.Contains(word1[stressPos1 - 1]))
					pair.baseCons1 = word1[stressPos1 - 1];
				if (consonants_strong.Contains(word2[stressPos2 - 1]) || consonants_soft.Contains(word2[stressPos2 - 1]))
					pair.baseCons2 = word2[stressPos2 - 1];
			}
			AssignRhymeTypes(pair);
		}

		public async Task<double> ScoreRhyme(string line1, string line2)
		{
			#region [Variables Init]
			RhymePair pair = new RhymePair();
			double score = 0;

			int length1 = Regex.Matches(line1, @"[уеыаоэяиёю]", RegexOptions.IgnoreCase).Count;
			int length2 = Regex.Matches(line2, @"[уеыаоэяиёю]", RegexOptions.IgnoreCase).Count;

			string[] lineSep1 = line1.Split(' ');
			string[] lineSep2 = line2.Split(' ');
			string word1 = "", word2 = "";
			for(int i = lineSep1.Length - 1; i >= 0; i--)
			{
				string a = CleanPunctuation(lineSep1[i].ToLower());
				if (a != "")
				{
					word1 = a;
					break;
				}
			}
			for (int i = lineSep2.Length - 1; i >= 0; i--)
			{
				string a = CleanPunctuation(lineSep2[i].ToLower());
				if (a != "")
				{
					word2 = a;
					break;
				}
			}
			if (word1 == "" || word2 == "")
				return -100;

			Word w1 = new Word(await DictionaryAPIManager.SearchWordInDictionary(YoToYe(word1)));
			Word w2 = new Word(await DictionaryAPIManager.SearchWordInDictionary(YoToYe(word2)));
			//TODO: Необходимо создать класс Stress, который бы определял и сравнивал все возможные ударения
			if (!w1.SuccessParse || !w2.SuccessParse)
				return -100;

			if (w1 == null || w2 == null)
				return -100;

			int stressPos1 = w1.StressPosition;
			int stressPos2 = w2.StressPosition;
			int stress1 = RecognizeStressSyllable(word1, stressPos1);
			int stress2 = RecognizeStressSyllable(word2, stressPos2);

			pair.speechPart1 = w1.speechPart;
			pair.speechPart2 = w2.speechPart;
			pair.word1 = word1;
			pair.word2 = word2;

			word1 = Simplificate(word1);
			word2 = Simplificate(word2);

			pair.stressPosition = stress1;
			SetRhymePair(word1, word2, stressPos1, stressPos2, pair);
			#endregion

			if (HasRhyme(word1, word2, stress1, stress2, stressPos1, stressPos2, pair))
			{
				score -= IsVerbishRhyme(pair) ? 1 : 0;
				score -= IsOnlyStressRhyme(pair) ? 1 : 0;
				score -= IsSimpleRhyme(w1, w2) ? 1 : 0;

				score += (IsDifferentSpeechPart(pair) || IsNonRhymingLemms(w1, w2, pair)) ? 1 : 0;
				score += IsRichRhyme(pair) ? 1 : 0;
				score += IsSameLength(length1, length2) ? 1 : -1;
				score += IsExoticWord(w1) ? 1 : 0;
			}
			else return -2;

			return score;
		}
	}
}
