using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PoetryApp.Models
{
	public class GenerationAPI
	{
		public static async Task<string> GeneratePorfire(string text)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://pelevin.gpt.dobro.ai/generate/");
			request.ServerCertificateValidationCallback = delegate { return true; };
			request.Method = "POST";
			request.UserAgent = "Chrome";
			request.ContentType = "application/json";

			string payload = "{\"prompt\": \""+text+"\", \"length\": 10}";

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
						return JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(res)["replies"][2];
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
