using System;
using System.Collections.Generic;
using System.Text;

namespace PoetryApp.Models
{
    public class Message
    {
        public string Text { get; set; }
        public bool isPlayer { get; set; }
        public string FromName { get; set; }

        public Message(string from, string text, bool isplayer = false)
        {
            Text = text;
            FromName = from;
            isPlayer = isplayer;
        }
    }
}
