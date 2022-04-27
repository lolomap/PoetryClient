using PoetryApp.Models;
using PoetryApp.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PoetryApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public MainViewModel()
        {
            Title = "Стихоплёт";

            PlayBotStartCommand = new Command(OnPlayBotStart);
            //OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://aka.ms/xamarin-quickstart"));
        }

        public ICommand PlayBotStartCommand { get; }
        //public ICommand OpenWebCommand { get; }

        private async void OnPlayBotStart(object obj)
        {
            await Shell.Current.GoToAsync(nameof(PlayBotPage));
        }
    }
}