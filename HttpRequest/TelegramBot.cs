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
using HttpRequest.Config;

namespace HttpRequest
{
    class TelegramBot
    {
        private static TelegramEnCommunicator TelEnComm;
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
            if (message.Text.StartsWith("&&"))
            {
                try
                {
                    if (TelEnComm == null)
                        TelEnComm = new TelegramEnCommunicator();

                    // Get the codes
                    String codes = message.Text.Substring(2);

                    TelEnComm.QueueCodes(codes);
                }
                catch (Exception ex)
                {
                    TelegramBot.SendCodeResult("Ooops, something went wrong in TelegramBot '&&' action " + ex.Message);
                }
            }
            else if (message.Text.StartsWith("&"))
            {
                try
                {
                    if (TelEnComm == null)
                        TelEnComm = new TelegramEnCommunicator();

                    // Get the code
                    String code = message.Text.Substring(1);

                    TelEnComm.QueueCode(code);
                }
                catch (Exception ex)
                {
                    TelegramBot.SendCodeResult("Ooops, something went wrong in TelegramBot '&' action " + ex.Message);
                }
            }
            else if (message.Text.StartsWith("/c") || message.Text.StartsWith("/с")) /*russian and english 'c'*/
            {
                try
                {
                    if (TelEnComm == null)
                        TelEnComm = new TelegramEnCommunicator();

                    // Get the code
                    String code = message.Text.Substring(2);

                    TelEnComm.QueueCode(code);
                }
                catch (Exception ex)
                {
                    TelegramBot.SendCodeResult("Ooops, something went wrong in TelegramBot '&' action " + ex.Message);
                }
            }
            else if (message.Text.StartsWith("/sector"))
            {
                try
                {
                    if (TelEnComm == null)
                        TelEnComm = new TelegramEnCommunicator();

                    TelEnComm.ShowSectors();
                }
                catch (Exception ex)
                {
                    TelegramBot.SendCodeResult("Ooops, something went wrong in TelegramBot 'sector' action " + ex.Message);
                }
            }
            else if (message.Text.StartsWith("/monitor"))
            {
                try
                {
                    if (TelEnComm == null)
                        TelEnComm = new TelegramEnCommunicator();

                    TelEnComm.StartMonitoring();
                }
                catch (Exception ex)
                {
                    TelegramBot.SendCodeResult("Ooops, something went wrong in TelegramBot 'monitor' action " + ex.Message);
                }
            }
            else if (message.Text.StartsWith("/level"))
            {
                try
                {
                    if (TelEnComm == null)
                        TelEnComm = new TelegramEnCommunicator();

                    TelEnComm.GetCurrentLevel();
                }
                catch (Exception ex)
                {
                    TelegramBot.SendCodeResult("Ooops, something went wrong in TelegramBot 'level' action " + ex.Message);
                }
            }
            else if (message.Text.StartsWith("/setgameid"))
            {
                var id = message.Text.Substring(11);
                // Get the code
                Settings.GameUrl = id;

                SendCodeResult("Game id is set to " + id);
            }
            else if (message.Text.StartsWith("/setdomain"))
            {
                var domain = message.Text.Substring(11);
                // Set game domain (host)
                Settings.GameHost = domain;

                SendCodeResult("Game domain is set to " + domain);
            }
            else if (message.Text.StartsWith("/setlogin"))
            {
                var login = message.Text.Substring(10);
                // Set game domain (host)
                Settings.Login = login;

                SendCodeResult("User login is set to " + login);
            }
            else if (message.Text.StartsWith("/setpassword"))
            {
                var pass = message.Text.Substring(13);
                // Set game domain (host)
                Settings.Password = pass;

                SendCodeResult("User password is set to " + pass);
            }
            else if (message.Text.StartsWith("/setgamefullurl"))
            {
                var url = message.Text.Substring(16);
                // Set game domain (host)
                Settings.gameUrl = url;

                SendCodeResult("Game url is set to " + url);
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
            else if (message.Text.StartsWith("/help")
                || message.Text.StartsWith("/usage"))
            {
                var usage = @"Usage:
*>>> Main commands <<<*
& - send code
&& - send several codes separated by comma
/monitor - watch levels
/level - get current level
/sector - get all sectors
*>>> Configuration commands <<<*
setlogin - change bot login *НЕ ТРОГАЙ :)*
setpassword - change bot password *НЕ ТРОГАЙ :)*
setdomain - change EN domain *НЕ ТРОГАЙ :)*
setgameid - change game id *НЕ ТРОГАЙ :)*
setgamefullurl - change game url *НЕ ТРОГАЙ :)*
";

                await Bot.SendTextMessageAsync(message.Chat.Id, usage, ParseMode.Markdown);
            }
        }

        public static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            await Bot.AnswerCallbackQueryAsync(callbackQueryEventArgs.CallbackQuery.Id,
                $"Received {callbackQueryEventArgs.CallbackQuery.Data}");
        }

        public static async void SendCodeResult(string result)
        {
            try
            {
                if (result != "" && result != null && MainChatId != 0)
                    await Bot.SendTextMessageAsync(MainChatId, result, ParseMode.Markdown);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception @ TelegramBot.SendCodeResult() " + ex.Message);
            }
        }

    }
}
