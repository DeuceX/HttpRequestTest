using HtmlAgilityPack;
using System;
using HapCss;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace HttpRequest
{
    class HtmlParser
    {
        private static HtmlDocument doc = new HtmlDocument();

        public static Dictionary<string, string> GenerateLevelInfoFromPage(string html, string code, bool needContent)
        {
            doc.LoadHtml(html);

            var levelInfo = new Dictionary<string, string>();

            levelInfo.Add("code", code);
            levelInfo.Add("isCorrect", IsCorrect());
            levelInfo.Add("LevelId", GetLevelId());
            levelInfo.Add("LevelNumber", GetLevelNumber());

            if (needContent)
            {
                levelInfo.Add("Content", GetLevelContent());
                levelInfo.Add("Coordinates", GetCoordinates(levelInfo["Content"]));
            }

            return levelInfo;
        }

        private static string GetCoordinates(string content)
        {
            string pattern = "[0-9][0-9][.,][0-9][0-9][0-9]+,? ?[0-9][0-9][.,][0-9][0-9][0-9]+";
            Regex rgx = new Regex(pattern);

            return rgx.Match(content).ToString();
        }

        private static string IsCorrect()
        {
            return (doc.QuerySelector("#incorrect") == null) 
                ? (doc.QuerySelector("li.correct") != null) ? "true" : "error"
                : "false";
        }

        private static string GetLevelId()
        {
            return doc.QuerySelector("input[name=\"LevelId\"]").GetAttributeValue("value", "");
        }

        private static string GetLevelNumber()
        {
            return doc.QuerySelector("input[name=\"LevelNumber\"]").GetAttributeValue("value", ""); ;
        }

        private static string GetLevelContent()
        {
            var s = doc.QuerySelector("div.content").InnerHtml;

            s = s.Substring(9);

            var keys = new Dictionary<string, string>();


            keys.Add("<!--end cols-->", String.Empty);
            keys.Add("<!--end cols-wrapper -->", String.Empty);
            keys.Add("\r\n\t\r\n\t\r\n\t\t", String.Empty);
            keys.Add("\\t", String.Empty);
            keys.Add("\\n", String.Empty);
            keys.Add("&nbsp;", " ");
            keys.Add("<h1>", "*");
            keys.Add("</h1>", "* \n");
            keys.Add("<h2>", "*");
            keys.Add("</h2>", "* \n");
            keys.Add("<h3>", "*");
            keys.Add("</h3>", "* \n");
            keys.Add("<strong>", "*");
            keys.Add("</strong>", "*");
            keys.Add("<span>", String.Empty);
            keys.Add("</span>", String.Empty);
            keys.Add("<p>", String.Empty);
            keys.Add("</p>", "\n");
            keys.Add("<br>", "\n");
            keys.Add("<span.+?>", String.Empty);
            keys.Add("<div.+?>", String.Empty);
            keys.Add("</div>", "\n");
            keys.Add("</script>", String.Empty);
            keys.Add("<script.+>", String.Empty);
            keys.Add("<h3.+?>", "*");
            keys.Add("<hr>", "--------------------------------");
            keys.Add("<b>", "*");
            keys.Add("</b>", "*");
            keys.Add("[*]Автопереход[*]", "Автопереход");

            Regex rgx;

            foreach (var key in keys)
            {
                rgx = new Regex(key.Key);
                s = rgx.Replace(s, key.Value);
            }

            // Delete script for timeout
            if (s.Contains("//<![CDATA["))
            {
                s = s.Substring(0, s.IndexOf("//<![CDATA[")) + s.Substring(s.IndexOf("//]]>") + 5);
                if (s.Contains("//<![CDATA["))
                {
                    s = s.Substring(0, s.IndexOf("//<![CDATA[")) + s.Substring(s.IndexOf("//]]>") + 5);
                }
            }

            return s;
        }

    }
}
