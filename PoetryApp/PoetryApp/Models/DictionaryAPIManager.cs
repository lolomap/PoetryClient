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
		public static string url = "http://62.113.110.236/";

		public static async Task<string> SearchWordInDictionary(string word)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "api/Dict/" + word);
			request.Method = "GET";
			request.ContentType = "application/json; charset=utf-8";

			while (true)
			{
				try
				{
					using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
					using (Stream stream = response.GetResponseStream())
					using (StreamReader reader = new StreamReader(stream))
					{
						string data = await reader.ReadToEndAsync();
						if (data[0] != '{' && data != "None")
							continue;
						return data;
					}
				}
				catch
				{
					return "None";
				}
			}
		}

		public static async Task<string> SpellCheck(string text)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "api/Spellcheck/" + text);
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
	}
}
