using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputMessageContents;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;

namespace HttpRequest
{
    class TelegramBot
    {
        private static TelegramEnCommunicator TelEnComm = new TelegramEnCommunicator();
        private static readonly TelegramBotClient Bot = new TelegramBotClient("215890940:AAHvKbm07EZyovI6TSLPyrxz3WNQ9DDJe9Q");
        private static long MainChatId = 0;

        public static TelegramBotClient TeleBot {
            get {
                Bot.OnCallbackQuery += TelegramBot.BotOnCallbackQueryReceived;
                Bot.OnMessage += TelegramBot.BotOnMessageReceived;
                Bot.OnMessageEdited += TelegramBot.BotOnMessageReceived;
                Bot.OnInlineQuery += TelegramBot.BotOnInlineQueryReceived;
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

        public static async void BotOnInlineQueryReceived(object sender, InlineQueryEventArgs inlineQueryEventArgs)
        {
            InlineQueryResult[] results = {
                new InlineQueryResultLocation
                {
                    Id = "1",
                    Latitude = 40.7058316f, // displayed result
                    Longitude = -74.2581888f,
                    Title = "New York",
                    InputMessageContent = new InputLocationMessageContent // message if result is selected
                    {
                        Latitude = 40.7058316f,
                        Longitude = -74.2581888f,
                    }
                },

                new InlineQueryResultLocation
                {
                    Id = "2",
                    Longitude = 52.507629f, // displayed result
                    Latitude = 13.1449577f,
                    Title = "Berlin",
                    InputMessageContent = new InputLocationMessageContent // message if result is selected
                    {
                        Longitude = 52.507629f,
                        Latitude = 13.1449577f
                    }
                }
            };

            await Bot.AnswerInlineQueryAsync(inlineQueryEventArgs.InlineQuery.Id, results, isPersonal: true, cacheTime: 0);
        }

        public static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;

            MainChatId = message.Chat.Id;

            if (message == null || message.Type != MessageType.TextMessage) return;

            // Recieve code
            if (message.Text.StartsWith("&"))
            {
                // Get the code
                String code = message.Text.Substring(1);

                TelEnComm.QueueCode(code);
            }
            else if (message.Text.StartsWith("/monitor"))
            {
                TelEnComm.StartMonitoring();
            }
            else if (message.Text.StartsWith("/level"))
            {
                TelEnComm.GetCurrentLevel();
            }
            else if (message.Text.StartsWith("/photo")) // send a photo
            {
                await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

                const string file = @"<FilePath>";

                var fileName = file.Split('\\').Last();

                using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var fts = new FileToSend(fileName, fileStream);

                    await Bot.SendPhotoAsync(message.Chat.Id, fts, "Nice Picture");
                }
            }
            else
            {
                var usage = @"Usage:
& - send code
/monitor - watch levels
/level - get current level
";

                await Bot.SendTextMessageAsync(message.Chat.Id, usage);
            }
        }

        public static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            await Bot.AnswerCallbackQueryAsync(callbackQueryEventArgs.CallbackQuery.Id,
                $"Received {callbackQueryEventArgs.CallbackQuery.Data}");
        }

        public static async void SendCodeResult(string result)
        {
            await Bot.SendTextMessageAsync(MainChatId, result, ParseMode.Markdown);
        }

    }
}
