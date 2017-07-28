using System;
using Telegram.Bot;

namespace HttpRequest
{
    class Program
    {
        private static TelegramBotClient Bot = TelegramBot.TeleBot;

        public static void Main(string[] args)
        {
            var me = Bot.GetMeAsync().Result;

            Console.Title = me.Username;

            Bot.StartReceiving();
            Console.ReadLine();
            Bot.StopReceiving();
        }        
    }
}
