using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static PoetryApp.Models.Word;

namespace PoetryApp.Models
{
	public class GenerationAPI
	{
		public static async Task<string> GenerateSimple()
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://62.113.110.236/py/");
			//request.ServerCertificateValidationCallback = delegate { return true; };
			request.Method = "GET";
			//request.UserAgent = "Chrome";
			request.ContentType = "application/json; charset=utf-8";

			int i = 0;
			while (i < 5)
			{
				try
				{

					using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
					using (Stream stream = response.GetResponseStream())
					using (StreamReader reader = new StreamReader(stream))
					{
						string res = await reader.ReadToEndAsync();
						Dictionary<string, string> json = JsonConvert.DeserializeObject<Dictionary<string, string>>(res);
						return json["text"];
					}
				}
				catch
				{
					i++;
					continue;
				}
			}
			return "";
		}

		public static async Task<string> GenerateRhyme(string text, int speechPart)
		{
			string[] words = text.Split(' ');
			text = words[words.Length - 1];

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://62.113.110.236/py/");
			//request.ServerCertificateValidationCallback = delegate { return true; };
			request.Method = "POST";
			//request.UserAgent = "Chrome";
			request.ContentType = "application/json";

			string payload = "{\"data\": \"" + text + "\", \"pos\": 1, \"speech_part\": " + (speechPart).ToString() + "}";

			using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
			{
				streamWriter.Write(payload);
			}

			int i = 0;
			while (i < 5)
			{
				try
				{

					using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
					using (Stream stream = response.GetResponseStream())
					using (StreamReader reader = new StreamReader(stream))
					{
						string res = await reader.ReadToEndAsync();
						Dictionary<string, string> json = JsonConvert.DeserializeObject<Dictionary<string, string>>(res);
						return json["text"];
					}
				}
				catch
				{
					i++;
					continue;
				}
			}
			return "";
		}

		public static async Task<string> GeneratePorfire(string text)
		{
			text = text.Replace("\n", "\\n");
			//text = text.Replace("\"", "″");

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://poetry.gpt.dobro.ai/generate/");
			request.ServerCertificateValidationCallback = delegate { return true; };
			request.Method = "POST";
			request.UserAgent = "Chrome";
			request.ContentType = "application/json";

			string payload = "{\"prompt\": \"" + text + "\", \"length\": 20}";

			using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
			{
				streamWriter.Write(payload);
			}

			int i = 0;
			while (i < 5)
			{
				try
				{

					using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
					using (Stream stream = response.GetResponseStream())
					using (StreamReader reader = new StreamReader(stream))
					{
						string res = await reader.ReadToEndAsync();
						Dictionary<string, List<string>> json = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(res);
						foreach (string sample in json["replies"])
						{
							if (sample.Contains("\n"))
								return sample.Split('\n')[1];
						}
						return json["replies"][0];
					}
				}
				catch
				{
					i++;
					continue;
				}
			}
			return "";
		}
	}
}
