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
    public class PlayBotViewModel : BaseViewModel
    {
        public string SendMessageInputText { set; get; }
        public ObservableCollection<Message> Messages { get; }

        public PlayBotViewModel()
        {
            Title = "Игра с ботом";
            Messages = new ObservableCollection<Message>();
            SendMessageCommand = new Command(OnSendMessage);

            Messages.Add(new Message("Поэт", "Приветствую, игрок!"));
        }

        public ICommand SendMessageCommand { get; }

        private void OnSendMessage(object obj)
        {
            Messages.Add(new Message("Игрок", SendMessageInputText, true));
            //SendMessageInputText = "";
        }
    }
}