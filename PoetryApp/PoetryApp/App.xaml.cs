using PoetryApp.Models;
using PoetryApp.Services;
using PoetryApp.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PoetryApp
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
			if (Application.Current.Properties.ContainsKey("username") && Application.Current.Properties.ContainsKey("password"))
			{
				Account.Login(Application.Current.Properties["username"] as string, Application.Current.Properties["password"] as string, true);
			}
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
