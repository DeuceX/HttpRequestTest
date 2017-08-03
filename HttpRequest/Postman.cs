using System;
using System.IO;
using System.Net;
using System.Text;
using HttpRequest.Config;
using System.Collections.Generic;

namespace HttpRequest
{
    class Postman
    { 
        public Postman()
        {
            ReassignCookies();
            CreateRequest();
        }

        // Main request for all communication
        private HttpWebRequest request;
        // Login attempts, if it reaches 10 - program stops with error
        private int loginAttempts = 0;
        private CookieCollection cookies;

        // Send Request to EN game
        public Dictionary<string, string> SendRequest(string code, int levelId, int levelNumber, bool needContent)
        {
            CreateRequest();

            // Generate the answer
            var postData = "------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"LevelAction.Answer\"\r\n\r\n" 
                + code + "\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"LevelId\"\r\n\r\n" 
                + levelId + "\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"LevelNumber\"\r\n\r\n" 
                + levelNumber + "\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW--";
            var data = Encoding.UTF8.GetBytes(postData);

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            
            // Relogin if redirected to login page
            if (response.ResponseUri.ToString().Contains("login")
                || responseString.Contains("Login.aspx?"))
            {
                if (loginAttempts < 6)
                {
                    loginAttempts++;
                    ReassignCookies();
                    CreateRequest();
                    return SendRequest(code, levelId, levelNumber, needContent);
                }
                else
                {
                    throw new Exception("Cannot login to EN web-site");
                }
            }
            
            loginAttempts = 0;

            // Get level info
            return HtmlParser.GenerateLevelInfoFromPage(responseString, code, needContent);
        }

        /// <summary>
        /// Adds headers and tokens to main request
        /// </summary>
        private void CreateRequest()
        {
            request = (HttpWebRequest)WebRequest.Create(Settings.GameUrl);

            request.Method = "POST";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36";
            request.ContentType = "multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.Host = Settings.GameHost;
            request.KeepAlive = true;
            request.CookieContainer = new CookieContainer();

            request.CookieContainer.Add(cookies);
        }

        /// <summary>
        /// Get new cookies and assign them to cookies variable
        /// </summary>
        private void ReassignCookies()
        {
            cookies = GetAuthCookies();
        }

        /// <summary>
        /// Login to Web site and return cookies
        /// </summary>
        /// <returns>CoockieCollection</returns>
        private CookieCollection GetAuthCookies()
        {
            var request = (HttpWebRequest)WebRequest.Create(Settings.SiteUrl);

            // Add headers to request
            request.Method = "POST";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36";
            request.ContentType = "multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.Host = Settings.GameHost;
            request.CookieContainer = new CookieContainer();
            request.Headers.Add("cache-control", "no-cache");
            request.Headers.Add("accept-language", "RU,ru;q=0.8,en-US;q=0.6,en;q=0.4,uk;q=0.2,sr;q=0.2,de;q=0.2");
            request.Headers.Add("accept-encoding", "gzip, deflate");
            request.AllowAutoRedirect = false;

            // Fill authorization form
            var postData = "------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"socialAssign\"\r\n\r\n0\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"Login\"\r\n\r\n"
                + Settings.Login + "\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"Password\"\r\n\r\n"
                + Settings.Password + "\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"EnButton1\"\r\n\r\nВход\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"ddlNetwork\"\r\n\r\n1\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW--";
            var data = Encoding.ASCII.GetBytes(postData);

            // Send request and read response
            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return response.Cookies;
        }
    }
}
