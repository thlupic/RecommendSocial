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
        //dobiva FB ID
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
                    var IMDBFBLink = String.Format("http://www.imdb.com{0}", FBText);
                    return IMDBFBLink;
                    //var webmethods = new Methods();
                    //string FBHTML = webmethods.GET(IMDBFBLink);

                    //var index = FBHTML.IndexOf("pageID");

                    //var stringnew = FBHTML.Substring(index, 50);
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

        //scrapea frendove
        public string getFacebookFriends(string facebookPageID)
        {
            facebookPageID = "564976613593712";
            string URI = String.Format("https://www.facebook.com/browse/friended_fans_of/?page_id={0}", facebookPageID);

            string html = new WebClient().DownloadString(URI);

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            HtmlNode docNode = htmlDoc.DocumentNode;
            var node = docNode.SelectSingleNode("//div[id=\"u_0_0\"]");

            return null;
        }
    }
}
