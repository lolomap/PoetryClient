using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PoetryApp.Models
{
	public class DictionaryAPIManager
	{
		public static string url = "https://poetry-api-l.herokuapp.com/";
		//static SymSpell spellchecker = new SymSpell(100000, 2);

		//public static void LoadSpellChecker()
		//{
		//	Assembly assembly = IntrospectionExtensions.GetTypeInfo(typeof(DictionaryAPIManager)).Assembly;
		//	using (Stream stream = assembly.GetManifestResourceStream("PoetryApp.ru-100k.txt"))
		//	{
		//		if (!spellchecker.LoadDictionary(stream, 0, 1))
		//			throw new Exception("Load Spell Checker failed");
		//	}
		//}

		public static async Task<string> SearchWordInDictionary(string word)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "api/Dict/" + word);
			request.Method = "GET";
			request.ContentType = "application/json; charset=utf-8";

			try
			{
				using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
				using (Stream stream = response.GetResponseStream())
				using (StreamReader reader = new StreamReader(stream))
				{
					return await reader.ReadToEndAsync();
				}
			}
			catch
			{
				return "None";
			}
		}

		//public static string SpellCheck(string text)
		//{
		//	List<SymSpell.SuggestItem> s = spellchecker.LookupCompound(text, 1);

		//	return s[0].term;
		//}
	}
}
