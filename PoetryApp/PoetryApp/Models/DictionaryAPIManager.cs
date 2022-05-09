using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PoetryApp.Models
{
	public class DictionaryAPIManager
	{
		public static string url = "https://poetry-api-l.herokuapp.com/";

		public static async Task<string> SearchWordInDictionary(string word)
		{
			//string result = await REQUESTING
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "api/Dict/" + word);
			request.Method = "GET";
			request.ContentType = "application/json; charset=utf-8";
			//request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

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
