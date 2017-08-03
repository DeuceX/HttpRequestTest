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
        private Random rnd = new Random();

        public TelegramEnCommunicator ()
        {
            _postMan = new Postman();
            SendEmptyCode(false);

            Thread t = new Thread(MonitorCodes);
            t.Start();
        }

        public void QueueCode(string code)
        {
            _codes.Enqueue(code.Trim());
        }

        public void QueueCodes(string codes)
        {
            string[] arr = codes.Split(',');
            for (int i = 0; i < arr.Length; i++)
            {
                _codes.Enqueue(arr[i].Trim());
            }
        }

        public void StartMonitoring()
        {
            Thread t = new Thread(MonitorLevels);
            t.Start();
        }

        public void GetCurrentLevel()
        {
            LevelInfoSent = -1;
        }

        public void ShowSectors()
        {
            Dictionary<string, string> result = SendEmptyCode(true);

            string sectors = result["Sectors"];

            TelegramBot.SendCodeResult((sectors == "") ? "На уровне всего 1 сектор!" : sectors);
        }
        
        private void MonitorCodes()
        {
            while (true)
            {
                SendCode();
                Thread.Sleep(500 + rnd.Next(10, 100));
            }
        }

        private void MonitorLevels()
        {
            while (true)
            {
                try
                {
                    var result = SendEmptyCode(true);

                    if (LevelInfoSent == Int32.Parse(result["LevelNumber"]))
                    {
                        Thread.Sleep(2500);
                        continue;
                    }

                    TelegramBot.SendCodeResult(result["Content"]);
                    if (result["Coordinates"] != "" || result["Coordinates"] != null)
                    {
                        TelegramBot.SendCodeResult(result["Coordinates"]);
                    }

                    LevelInfoSent = Int32.Parse(result["LevelNumber"]);

                    Thread.Sleep(2500);
                }
                catch (Exception ex)
                {
                    TelegramBot.SendCodeResult("Error in Monitor thread, restarting... " + ex.Message);
                }
            }
        }

        private void SendCode()
        {
            try
            {
                if (_codes.Count() > 0)
                {
                    var code = _codes.Dequeue();

                    if (_codesArchive.Contains(code))
                    {
                        TelegramBot.SendCodeResult("Код *" + code + "* уже был \U0001F61C");
                        return;
                    }

                    _codesArchive.Add(code);

                    var result = _postMan.SendRequest(code, Settings.LevelId, Settings.LevelNumber, false);

                    string isCorrect = (result["isCorrect"] == "true") ? "верный \U0001F60D" : "не верный \U0001F614";
                    TelegramBot.SendCodeResult("Код *" + code + "* " + isCorrect);

                    UpdateLevelNumberAndId(result);
                }
            }
            catch (Exception ex)
            {
                TelegramBot.SendCodeResult("Ooops, something went wrong while " +
                    "sending code @ TelegramEnCommunicator.SendCode(): " + ex.Message);
            }
        }

        /// <summary>
        /// Method to update LevelId and LevelNumber
        /// </summary>
        private Dictionary<string, string> SendEmptyCode(bool needContent)
        {
            var result = _postMan.SendRequest(String.Empty, Settings.LevelId, Settings.LevelNumber, needContent);
            UpdateLevelNumberAndId(result);

            return result;
        }
        
        private void UpdateLevelNumberAndId(Dictionary<string, string> result)
        {
            try
            {
                int LevelNumber = Int32.Parse(result["LevelNumber"]);
                int LevelId = Int32.Parse(result["LevelId"]);

                if (LevelNumber > Settings.LevelNumber)
                {
                    /*
                    if (Settings.LevelNumber != 1)
                        TelegramBot.SendCodeResult("Хороши-картоши! Новый уровень!" +
                            " \U0001F389\U0001F389\U0001F389\U0001F389\U0001F389");
                    */
                    _codes.Clear();
                    _codesArchive.Clear();
                }

                Settings.LevelNumber = LevelNumber;
                Settings.LevelId = LevelId;
            }
            catch (Exception ex)
            {
                TelegramBot.SendCodeResult("Ooops, something went wrong while parsing LevelNumber " +
                    "and LevelId from Dictionary @ TelegramENCommunicator Class. Check: " + ex.Message);
            }
        }
    }
}
