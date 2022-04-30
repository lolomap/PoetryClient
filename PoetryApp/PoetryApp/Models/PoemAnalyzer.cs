using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoetryApp.Models
{
	public class RhymePair
	{
		public enum Type
		{
			
		}
		public string word1, word2;
		public List<Type> types;
		public double score = 0;
		public int stressPosition;
	}

	public class PoemAnalyzer
	{
		public enum RhymeMode
		{
			LastWord,
		};


		public PoemAnalyzer()
		{
		}


		public void FootByPoem(string poem)
		{
			//Определяем размер стиха
		}

		public int RecognizeStress(string word)
		{
			//Здесь мы с помощью сайта где-ударение.рф/в-слове- получаем положение ударения
			return 0;
		}

		public void AssignRhymeTypes(RhymePair pair)
		{
			string a = pair.word1, b = pair.word2;
			//https://ru.wikipedia.org/wiki/%D0%A0%D0%B8%D1%84%D0%BC%D0%B0
		}

		public double ScoreRhyme()
		{
			//https://works.doklad.ru/view/uj7fSRE4QfU.html
			return 0;
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
	}
}
