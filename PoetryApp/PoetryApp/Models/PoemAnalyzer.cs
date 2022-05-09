using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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
		public enum RhymeMode
		{
			LastWord,
		};

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

		Dictionary<string, char> replace = new Dictionary<string, char>()
		{
			{ "тс", 'ц' },
			{ "тч", 'ч' },
			{ "тш", 'ш' },
			{ "тц", 'ц' },
			{ "тщ", 'щ' },
			{ "сч", 'щ' },
		};

		WordAnalyzer wanalyzer = new WordAnalyzer();


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

			return a;
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

			return new Tuple<int, int>(0, 0);
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
			else if (pair.stressedPart1 == pair.stressedPart2 && pair.stressedPart1 != "")
			{
				pair.types.Add(RhymePair.Type.Poor);
			}
		}

		public async Task<double> ScoreRhyme(string word1, string word2)
		{
			//https://works.doklad.ru/view/uj7fSRE4QfU.html
			RhymePair pair = new RhymePair();
			double score = 0;

			word1 = word1.ToLower(); word2 = word2.ToLower();

			Word w1 = new Word(await DictionaryAPIManager.SearchWordInDictionary(YoToYe(word1)));
			Word w2 = new Word(await DictionaryAPIManager.SearchWordInDictionary(YoToYe(word2)));

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

			word1 = Simplificate(word1);
			word2 = Simplificate(word2);

			if (stress1 == stress2)
			{
				pair.stressPosition = stress1;

				word1 = word1.ToLower(); word2 = word2.ToLower();
				pair.word1 = word1; pair.word2 = word2;

				if (stressPos1 > 0 && stressPos2 > 0)
				{
					if (consonants_strong.Contains(word1[stressPos1 - 1]) || consonants_soft.Contains(word1[stressPos1 - 1]))
						pair.baseCons1 = word1[stressPos1 - 1];
					if (consonants_strong.Contains(word2[stressPos2 - 1]) || consonants_soft.Contains(word2[stressPos2 - 1]))
						pair.baseCons2 = word2[stressPos2 - 1];
				}

				if (stressPos1 < word1.Length && stressPos2 < word2.Length)
				{
					pair.stressedPart1 = word1.Substring(stressPos1 + 1);
					pair.stressedPart2 = word2.Substring(stressPos2 + 1);
				}


				AssignRhymeTypes(pair);

				if (VowelsEquality(word1[stressPos1], word2[stressPos2]))
				{
					if (!pair.types.Contains(RhymePair.Type.Rich) && !pair.types.Contains(RhymePair.Type.Poor))
						score -= 1;
					if (pair.types.Contains(RhymePair.Type.Rich))
						score += 1;
					if (pair.types.Contains(RhymePair.Type.Poor))
						score += 0.5;

					if (pair.speechPart1 != pair.speechPart2)
						score += 1;
					if (pair.speechPart1 == pair.speechPart2 && pair.speechPart1 == Word.SpeechPart.Verb)
						score -= 1;

				}
				else
				{
					return -1.5;
				}
			}
			else
				return -5;

			return score;
		}

		public double CalculateRhyme(string line1, string line2, RhymeMode mode)
		{
			switch (mode)
			{
				case RhymeMode.LastWord:
					return 0;
				default:
					return 0;
			}
		}




		private static bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			//Return true if the server certificate is ok
			if (sslPolicyErrors == SslPolicyErrors.None)
				return true;

			bool acceptCertificate = true;
			string msg = "The server could not be validated for the following reason(s):\r\n";

			//The server did not present a certificate
			if ((sslPolicyErrors &
				 SslPolicyErrors.RemoteCertificateNotAvailable) == SslPolicyErrors.RemoteCertificateNotAvailable)
			{
				msg = msg + "\r\n    -The server did not present a certificate.\r\n";
				acceptCertificate = false;
			}
			else
			{
				//The certificate does not match the server name
				if ((sslPolicyErrors &
					 SslPolicyErrors.RemoteCertificateNameMismatch) == SslPolicyErrors.RemoteCertificateNameMismatch)
				{
					msg = msg + "\r\n    -The certificate name does not match the authenticated name.\r\n";
					acceptCertificate = false;
				}

				//There is some other problem with the certificate
				if ((sslPolicyErrors &
					 SslPolicyErrors.RemoteCertificateChainErrors) == SslPolicyErrors.RemoteCertificateChainErrors)
				{
					foreach (X509ChainStatus item in chain.ChainStatus)
					{
						if (item.Status != X509ChainStatusFlags.RevocationStatusUnknown &&
							item.Status != X509ChainStatusFlags.OfflineRevocation)
							break;

						if (item.Status != X509ChainStatusFlags.NoError)
						{
							msg = msg + "\r\n    -" + item.StatusInformation;
							acceptCertificate = false;
						}
					}
				}
			}

			//If Validation failed, present message box
			if (acceptCertificate == false)
			{
				msg = msg + "\r\nDo you wish to override the security check?";
				//          if (MessageBox.Show(msg, "Security Alert: Server could not be validated",
				//                       MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
				acceptCertificate = true;
			}

			return acceptCertificate;
		}
	}
}
