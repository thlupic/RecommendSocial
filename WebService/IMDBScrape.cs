using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;

namespace WebService
{
    public class IMDBScrape
    {
        public string getFacebookID(string IMDBID)
        {
            try
            {
                string URI = String.Format("http://www.imdb.com/title/tt{0}", IMDBID);

                string html = new WebClient().DownloadString(URI);

                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                HtmlNode docNode = htmlDoc.DocumentNode;
                HtmlNodeCollection nodes = docNode.SelectNodes("//div[id=\"titleDetails\"]");

                var singleNode = docNode.SelectSingleNode("//*[@id=\"titleDetails\"]");

                var nodewithSites = singleNode.ChildNodes[5];

                var divFacebook = nodewithSites.ChildNodes[3];

                var FBText = divFacebook.Attributes[0].Value;

                if (FBText.Contains("offsite-facebook"))
                {
                    return String.Format("http://www.imdb.com{0}", FBText);
                }

                if (nodes != null)
                {
                    var alert = "!!!";
                }
            }
            catch (Exception e)
            {
                var ex = e;
            }

            return null;
        }
    }
}
