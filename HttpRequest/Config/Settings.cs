using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpRequest.Config
{
    public abstract class Settings
    {
        private static string login = "stgr";
        private static string password = "gfvznm12";
        private static string siteUrl = "http://demo.en.cx/Login.aspx";
        private static string gameUrl = "http://demo.en.cx/gameengines/encounter/play/27060";
        private static int levelId = -1;
        private static int levelNumber = 1;

        public static string GameHost = "demo.en.cx";

        public static int LevelId
        {
            get { return Settings.levelId; }
            set { Settings.levelId = value; }
        }

        public static int LevelNumber
        {
            get { return Settings.levelNumber; }
            set { Settings.levelNumber = value; }
        }

        public static string Login
        {
            get { return Settings.login; }
            set { Settings.login = value; }
        }

        public static string Password
        {
            get { return Settings.password; }
            set { Settings.password = value; } }

        public static string SiteUrl
        {
            get { return Settings.siteUrl; }
            set { Settings.siteUrl = value; }
        }

        public static string GameUrl
        {
            get { return Settings.gameUrl; }
            set { Settings.gameUrl = value; }
        }
    }
}
