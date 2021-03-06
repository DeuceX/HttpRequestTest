﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpRequest.Config
{
    public abstract class Settings
    {
        private static string login = "stgr";
        private static string gameHost = "quest.ua";
        private static string password = "gfhjkm12";
        private static string siteUrl = "http://" + gameHost + "/Login.aspx";
        public static string gameUrl = "http://quest.ua/gameengines/encounter/play/59777";
        private static int levelId = -1;
        private static int levelNumber = 1;

        public static string GameHost
        {
            get { return Settings.gameHost; }
            set {
                Settings.gameHost = value;
                Settings.siteUrl = "http://" + value + "/Login.aspx";
            }
        }

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
            set {
                Settings.gameUrl = "http://" + gameHost + "/gameengines/encounter/play/" + value;
            }
        }
    }
}
