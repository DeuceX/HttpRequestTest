using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace QB.Telegram
{
    public class TelegramBot
    {
        private static readonly TelegramBotClient Bot = new TelegramBotClient("215890940:AAHvKbm07EZyovI6TSLPyrxz3WNQ9DDJe9Q");
        private static long MainChatId;

        public static TelegramBotClient TeleBot
        {
            get
            {
                Bot.OnCallbackQuery += TelegramBot.BotOnCallbackQueryReceived;
                Bot.OnMessage += TelegramBot.BotOnMessageReceived;
                Bot.OnMessageEdited += TelegramBot.BotOnMessageReceived;
                Bot.OnInlineResultChosen += TelegramBot.BotOnChosenInlineResultReceived;
                Bot.OnReceiveError += TelegramBot.BotOnReceiveError;

                return TelegramBot.Bot;
            }
        }

        public static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Debugger.Break();
        }

        public static void BotOnChosenInlineResultReceived(object sender, ChosenInlineResultEventArgs chosenInlineResultEventArgs)
        {
            Console.WriteLine($"Received choosen inline result: {chosenInlineResultEventArgs.ChosenInlineResult.ResultId}");
        }

        public static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;

            MainChatId = message.Chat.Id;

            if (message == null || message.Type != MessageType.TextMessage) return;

            // Recieve code
            if (message.Text.StartsWith("/help")
                || message.Text.StartsWith("/usage"))
            {
                var usage = @"Yo";

                await Bot.SendTextMessageAsync(message.Chat.Id, usage, ParseMode.Markdown);
            }
        }

        public static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            await Bot.AnswerCallbackQueryAsync(callbackQueryEventArgs.CallbackQuery.Id,
                $"Received {callbackQueryEventArgs.CallbackQuery.Data}");
        }

    }
}
