using System;
using System.Collections.Generic;
using HttpRequest.Config;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace HttpRequest
{
    public class TelegramEnCommunicator
    {
        private Postman _postMan;
        private Queue<string> _codes = new Queue<string>();
        private List<string> _codesArchive = new List<string>();
        private int LevelInfoSent = -1;

        public TelegramEnCommunicator ()
        {
            _postMan = new Postman();
            SendEmptyCode(false);
        }

        public void QueueCode(string code)
        {
            _codes.Enqueue(code);
            SendCode();
        }

        public void StartMonitoring()
        {
            Thread t = new Thread(Monitor);
            t.Start();
        }

        public void GetCurrentLevel()
        {
            LevelInfoSent = -1;
        }

        private void Monitor()
        {
            while (true)
            {
                var result = SendEmptyCode(true);

                if (LevelInfoSent == Int32.Parse(result["LevelNumber"]))
                {
                    Thread.Sleep(3000);
                    continue;
                }

                TelegramBot.SendCodeResult(result["Content"]);

                LevelInfoSent = Int32.Parse(result["LevelNumber"]);

                Thread.Sleep(3000);
            }
        }

        private void SendCode()
        {
            var code = _codes.Dequeue();

            if (_codesArchive.Contains(code))
            {
                TelegramBot.SendCodeResult("Код *" + code + "* уже был \U0001F61C");
                return;
            }

            _codesArchive.Add(code);

            var result = _postMan.SendRequest(code, Settings.LevelId, Settings.LevelNumber, false);

            UpdateLevelInfo(result);

            string isCorrect = (result["isCorrect"] == "true") ? "верный \U0001F60D" : "не верный \U0001F614";

            TelegramBot.SendCodeResult("Код *" + code + "* " + isCorrect);
        }

        /// <summary>
        /// Method to update LevelId and LevelNumber
        /// </summary>
        private Dictionary<string, string> SendEmptyCode(bool needContent)
        {
            var result = _postMan.SendRequest(String.Empty, Settings.LevelId, Settings.LevelNumber, needContent);
            UpdateLevelInfo(result);

            return result;
        }

        private void UpdateLevelInfo(Dictionary<string, string> result)
        {
            try
            {
                int LevelNumber = Settings.LevelNumber;
                int LevelId = Settings.LevelId;

                if (!Int32.TryParse(result["LevelNumber"], out LevelNumber)
                        || !Int32.TryParse(result["LevelId"], out LevelId))
                    return;

                if (LevelNumber > Settings.LevelNumber)
                {
                    _codes.Clear();
                    _codesArchive.Clear();
                }
                
                Settings.LevelNumber = LevelNumber;
                Settings.LevelId = LevelId;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ooops, something went wrong while parsing LevelNumber " +
                    "and LevelId from Dictionary @ TelegramENCommunicator Class. Check: " + ex.Message);
            }
        }
    }
}
