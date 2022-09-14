using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PoetryApp.Models
{
	public static class Account
	{
		public static string Password { get; set; }
		public static string Username { get; set; }
		public static User user { get; set; } = null;
		//public static User user { get; set; } = new User() { Name = "legatt" };

		public static async Task<int> Login(string username, string password)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://62.113.110.236/api/Login/");
			//request.ServerCertificateValidationCallback = delegate { return true; };
			request.Method = "POST";
			//request.UserAgent = "Chrome";
			request.ContentType = "application/json; charset=utf-8";

			password = HashPassword(password, username);
			string payload = "{\"username\": \"" + username + "\", \"password\": \"" + password + "\"}";

			using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
			{
				streamWriter.Write(Encoding.UTF8.GetString(Encoding.Default.GetBytes(payload)));
			}

			int i = 0;
			while (i < 100)
			{
				try
				{

					using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
					using (Stream stream = response.GetResponseStream())
					using (StreamReader reader = new StreamReader(stream))
					{
						string res = await reader.ReadToEndAsync();
						Dictionary<string, object> json = JsonConvert.DeserializeObject<Dictionary<string, object>>(res);

						if ((string)json["success"] == "true")
						{
							object user_data = json["user"];
							user = DeserializeUser(user_data.ToString());
							Password = password;
							Username = username;
						}
						else
						{
							return 1;
						}


						return 0;
					}
				}
				catch
				{
					i++;
					continue;
				}
			}

			return -1;
		}

		public static async Task<int> Register(string username, string password)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://62.113.110.236/api/Register/");
			//request.ServerCertificateValidationCallback = delegate { return true; };
			request.Method = "POST";
			//request.UserAgent = "Chrome";
			request.ContentType = "application/json; charset=utf-8";

			password = HashPassword(password, username);
			string payload = "{\"username\": \"" + username + "\", \"password\": \"" + password + "\"}";

			using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
			{
				streamWriter.Write(Encoding.UTF8.GetString(Encoding.Default.GetBytes(payload)));
			}

			int i = 0;
			while (i < 100)
			{
				try
				{

					using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
					using (Stream stream = response.GetResponseStream())
					using (StreamReader reader = new StreamReader(stream))
					{
						string res = await reader.ReadToEndAsync();

						return res == "true" ? 0 : 1;
					}
				}
				catch
				{
					i++;
					continue;
				}
			}

			return -1;
		}

		static User DeserializeUser(string json)
		{
			User user = new User();
			Dictionary<string, string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

			user.Name = data["name"];

			int sc, gcount;
			bool success = false;
			success = int.TryParse(data["score"], out sc);
			if (success)
			{
				user.TotalScore = sc;
			}

			success = int.TryParse(data["games"], out gcount);
			if (success)
			{
				user.GamesCount = gcount;
			}

			return user;
		}

		static string HashPassword(string password, string salt)
		{
			using(SHA256 sha256 = SHA256.Create())
			{
				byte[] text = Encoding.UTF8.GetBytes(password);
				byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
				byte[] textSalted = new byte[text.Length + saltBytes.Length];

				for (int i = 0; i < text.Length; i++)
				{
					textSalted[i] = text[i];
				}
				for (int i = 0; i < saltBytes.Length; i++)
				{
					textSalted[text.Length + i] = saltBytes[i];
				}

				byte[] hashedBytes = sha256.ComputeHash(textSalted);
				string hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
				return hash;
			}
		}
	}

	public class User
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int TotalScore { get; set; }
		public DateTime Created { get; set; }
		public int GamesCount { get; set; }
		public List<int> Achievements { get; set; }

	}
}